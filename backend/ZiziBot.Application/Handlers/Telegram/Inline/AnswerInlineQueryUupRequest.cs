using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Inline;

public class AnswerInlineQueryUupBotRequestModel : BotRequestBase
{
    public string? Query { get; set; }
}

public class AnswerInlineQueryUupRequestHandler : IRequestHandler<AnswerInlineQueryUupBotRequestModel, BotResponseBase>
{
    private readonly ILogger<AnswerInlineQueryUupRequestHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly UupDumpService _uupDumpService;

    public AnswerInlineQueryUupRequestHandler(ILogger<AnswerInlineQueryUupRequestHandler> logger, TelegramService telegramService, UupDumpService uupDumpService)
    {
        _logger = logger;
        _telegramService = telegramService;
        _uupDumpService = uupDumpService;
    }

    public async Task<BotResponseBase> Handle(AnswerInlineQueryUupBotRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        _logger.LogInformation("Find UUP for Query: {Query}", request.Query);

        var data = await _uupDumpService.GetUpdatesAsync(request.Query);

        var inlineQueryResults = data.Response.Builds
            .OrderByDescending(x => x.Created)
            .Where(x => !x.Title.Contains("Stack"))
            .Where(x => !x.Title.Contains("Cumulative"))
            .Select(build => {
                var htmlDescription = HtmlMessage.Empty
                    .TextBr(build.Title)
                    .TextBr(build.Created.ToString("yyyy-MM-dd HH:mm:ss tt"));

                var htmlContent = HtmlMessage.Empty
                    .Bold("Title: ").CodeBr(build.Title)
                    .Bold("Version: ").CodeBr(build.BuildNumber)
                    .Bold("Date: ").CodeBr(build.Created.ToString("yyyy-MM-dd HH:mm:ss tt"))
                    .Bold("Arch: ").CodeBr(build.Arch.ToString().ToUpper());

                var replyMarkup = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("⬇️ Download file", build.DownloadUrl)
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Pencarian baru", $"uup "),
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Pencarian lanjut", $"uup {request.Query}")
                    }
                });

                var fields = new InlineQueryResultArticle(
                    id: $"uup-{build.Uuid}",
                    title: $"{build.BuildNumber} - {build.Arch}",
                    inputMessageContent: new InputTextMessageContent(htmlContent.ToString())
                    {
                        ParseMode = ParseMode.Html,
                        DisableWebPagePreview = true
                    }
                )
                {
                    Description = htmlDescription.ToString(),
                    ReplyMarkup = replyMarkup
                };

                return fields;
            });

        return await _telegramService.AnswerInlineQueryAsync(inlineQueryResults);
    }

}