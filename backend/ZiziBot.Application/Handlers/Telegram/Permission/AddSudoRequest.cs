namespace ZiziBot.Application.Handlers.Telegram.Permission;

public class AddSudoRequestModel : RequestBase
{
    public long CustomUserId { get; set; }
}

[UsedImplicitly]
public class AddSudoRequestHandler : IRequestHandler<AddSudoRequestModel, ResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly SudoService _sudoService;

    public AddSudoRequestHandler(TelegramService telegramService, AppSettingsDbContext appSettingsDbContext, SudoService sudoService)
    {
        _telegramService = telegramService;
        _sudoService = sudoService;
    }

    public async Task<ResponseBase> Handle(AddSudoRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        await _telegramService.SendMessageText("Adding sudo user...");

        var serviceResult = await _sudoService.SaveSudo(new Sudoer()
        {
            UserId = request.CustomUserId == 0 ? request.UserId : request.CustomUserId,
            PromotedBy = request.UserId,
            PromotedFrom = request.ChatIdentifier,
            Status = (int)EventStatus.Complete
        });

        await _telegramService.EditMessageText(serviceResult.Message);

        return _telegramService.Complete();
    }
}