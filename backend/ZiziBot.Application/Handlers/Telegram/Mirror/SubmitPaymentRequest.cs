using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Mirror;

public class SubmitPaymentBotRequestModel : BotRequestBase
{
    public string? Payload { get; set; }
    public long ForUserId { get; set; }
}

public class SubmitPaymentRequestHandler : IRequestHandler<SubmitPaymentBotRequestModel, BotResponseBase>
{
    private readonly ILogger<SubmitPaymentRequestHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly MirrorDbContext _mirrorDbContext;
    private readonly AppSettingRepository _appSettingRepository;

    public SubmitPaymentRequestHandler(
        ILogger<SubmitPaymentRequestHandler> logger,
        TelegramService telegramService,
        AppSettingRepository appSettingRepository,
        MirrorDbContext mirrorDbContext
    )
    {
        _logger = logger;
        _telegramService = telegramService;
        _appSettingRepository = appSettingRepository;
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<BotResponseBase> Handle(SubmitPaymentBotRequestModel request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        _telegramService.SetupResponse(request);

        var cendolCount = 0;
        var transactionId = string.Empty;
        var userId = request.UserId;
        var expireDate = DateTime.UtcNow;

        var replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("Bagaimana cara mendapatkan OrderId?", UrlConst.DOC_MIRROR_VERIFY_DONATION)
            }
        });

        if (string.IsNullOrEmpty(request.Payload))
        {
            htmlMessage.Text("Sertakan tautan dari Trakteer.id untuk diverifikasi.").Br()
                .Bold("Contoh: ").CodeBr("/sp 9d16023b-67be-5a0b-bc47-47809f059013");


            return await _telegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
        }

        if (request.ForUserId != 0)
        {
            userId = request.ForUserId;
        }

        await _telegramService.SendMessageText("Sedang memverifikasi pembayaran. Silakan tunggu...");
        var trakteerParsedDto = await request.Payload.GetTrakteerApi();

        if (!trakteerParsedDto.IsValid)
        {
            htmlMessage.BoldBr("Pembayaran gagal diverifikasi.")
                .Text("Pastikan link yang kamu kirim benar dan bukti pembayaran sudah terverifikasi oleh Trakteer.");

            return await _telegramService.EditMessageText(htmlMessage.ToString());
        }

        var mirrorConfig = await _appSettingRepository.GetConfigSectionAsync<MirrorConfig>();

        if (trakteerParsedDto.OrderDate <= DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE).AddDays(-mirrorConfig!.PaymentExpirationDays))
        {
            return await _telegramService.EditMessageText("Bukti pembayaran sudah kadaluarsa. Silakan lakukan pembayaran ulang.");
        }

        var mirrorApproval = await _mirrorDbContext.MirrorApproval
            .Where(entity => entity.OrderId == trakteerParsedDto.OrderId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken);

        if (mirrorApproval != null)
        {
            htmlMessage = HtmlMessage.Empty
                .Bold("Pembayaran ini sudah diklaim.").Br()
                .Text("Pastikan <b>OrderId</b> Anda dapatkan dari web Trakteer.id");

            return await _telegramService.EditMessageText(htmlMessage.ToString(), replyMarkup);
        }

        _mirrorDbContext.MirrorApproval.Add(new MirrorApprovalEntity()
        {
            UserId = request.UserId,
            PaymentUrl = trakteerParsedDto.PaymentUrl,
            RawText = trakteerParsedDto.RawText,
            CendolCount = trakteerParsedDto.CendolCount,
            AdminFees = trakteerParsedDto.AdminFees,
            Subtotal = trakteerParsedDto.Total,
            OrderDate = trakteerParsedDto.OrderDate,
            PaymentMethod = trakteerParsedDto.PaymentMethod,
            OrderId = trakteerParsedDto.OrderId,
            Status = (int)EventStatus.Complete,
            TransactionId = transactionId
        });

        cendolCount = trakteerParsedDto.CendolCount;

        var mirrorUser = await _mirrorDbContext.MirrorUsers
            .Where(entity => entity.UserId == userId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        expireDate = DateTime.Now.AddMonths(cendolCount);

        if (mirrorUser == null)
        {
            _logger.LogInformation("Creating Mirror subscription for user {UserId} with Expire date: {Date}", userId, expireDate);

            _mirrorDbContext.MirrorUsers.Add(new MirrorUserEntity()
            {
                UserId = userId,
                ExpireDate = expireDate,
                Status = (int)EventStatus.Complete,
                TransactionId = transactionId
            });
        }
        else
        {
            _logger.LogInformation("Extending Mirror subscription for user {UserId} with Expire date: {Date}", userId, expireDate);

            expireDate = mirrorUser.ExpireDate < DateTime.Now
                ? expireDate // If expired, will be started from now
                : mirrorUser.ExpireDate.AddMonths(cendolCount); // If not expired, will be extended from expire date

            mirrorUser.ExpireDate = expireDate;
            mirrorUser.Status = (int)EventStatus.Complete;
            mirrorUser.TransactionId = transactionId;
        }

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        htmlMessage.Bold("Pengguna berhasil disimpan").Br()
            .Bold("ID Pengguna: ").Code(userId.ToString()).Br()
            .Bold("Pengguna: ").UserMention(request.User).Br()
            .Bold("Jumlah Cendol: ").Code(cendolCount.ToString()).Br()
            .Bold("Langganan sampai: ").Code(expireDate.AddHours(Env.DEFAULT_TIMEZONE).ToString("yyyy-MM-dd HH:mm:ss zzz")).Br();

        await _telegramService.EditMessageText(htmlMessage.ToString());

        htmlMessage.Bold("OrderID: ").CodeBr(trakteerParsedDto.OrderId)
            .Bold("Url: ").Text(trakteerParsedDto.PaymentUrl);

        return await _telegramService.SendMessageText(htmlMessage.ToString(), chatId: mirrorConfig.ApprovalChannelId, threadId: 0);
    }
}