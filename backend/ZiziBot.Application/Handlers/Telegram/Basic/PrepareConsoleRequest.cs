using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PrepareConsoleBotRequest : BotRequestBase
{
}

public class PrepareConsoleHandler : IRequestHandler<PrepareConsoleBotRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;

    public PrepareConsoleHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<BotResponseBase> Handle(PrepareConsoleBotRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var sessionId = Guid.NewGuid().ToString();
        var consoleUrl = EnvUtil.GetEnv(Env.WEB_CONSOLE_URL);
        var webUrlBase = consoleUrl + "?session_id=";
        var webUrl = webUrlBase + sessionId;

        if (!EnvUtil.IsEnvExist(Env.WEB_CONSOLE_URL))
        {
            await _telegramService.SendMessageText("Maaf fitur ini belum dipersiapkan");
        }

        var replyMarkup = InlineKeyboardMarkup.Empty();
        var htmlMessage = HtmlMessage.Empty
            .BoldBr("🎛 ZiziBot Console")
            .TextBr("Buka Console untuk mengelola pengaturan, catatan dan lain-lain.")
            .Br();

        if (webUrl.Contains("localhost"))
        {
            htmlMessage.Code(webUrl).Br();
        }
        else
        {
            replyMarkup = new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithLoginUrl("Buka Console", new LoginUrl()
                    {
                        Url = webUrl
                    })
                }
            }.ToButtonMarkup();
        }

        return await _telegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
    }
}