using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class GetIdBotRequestModel : BotRequestBase
{
}

public class GetIdRequestHandler : IRequestHandler<GetIdBotRequestModel, BotResponseBase>
{
    private readonly TelegramService _telegramService;

    public GetIdRequestHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<BotResponseBase> Handle(GetIdBotRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty;

        if (request.ChatType != ChatType.Private)
        {
            htmlMessage.BoldBr($"👥 {request.ChatTitle}")
                .Bold("Chat ID: ").CodeBr(request.ChatId.ToString());
        }

        if (request.Message?.IsTopicMessage ?? false)
        {
            htmlMessage.Br()
                .Bold("🧵 ").BoldBr(request.ReplyToMessage?.ForumTopicCreated?.Name)
                .Bold("Topic ID: ").CodeBr(request.ReplyToMessage?.MessageThreadId.ToString());
        }

        htmlMessage.Br()
            .BoldBr($"👤 {request.UserFullName}")
            .Bold("User ID: ").CodeBr(request.UserId.ToString());

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}