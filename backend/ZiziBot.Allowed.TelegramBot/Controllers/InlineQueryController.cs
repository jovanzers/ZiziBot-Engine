using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class InlineQueryController : CommandController
{
    private readonly ILogger<InlineQueryController> _logger;
    private readonly MediatorService _mediatorService;

    public InlineQueryController(ILogger<InlineQueryController> logger, MediatorService mediatorService)
    {
        _logger = logger;
        _mediatorService = mediatorService;
    }

    [InlineQuery()]
    public async Task InlineSearchBotApi(InlineQueryData data)
    {
        var inlineCmd = data.InlineQuery.Query.GetInlineQueryAt<string>(0);

        var result = inlineCmd switch {
            "api-doc" => await _mediatorService.EnqueueAsync(new AnswerInlineQueryApiDocBotRequestModel() {
                BotToken = data.Options.Token,
                InlineQuery = data.InlineQuery,
                Query = data.InlineQuery.Query.GetInlineQueryAt<string>(1)
            }),
            "uup" => await _mediatorService.EnqueueAsync(new AnswerInlineQueryUupBotRequestModel() {
                BotToken = data.Options.Token,
                InlineQuery = data.InlineQuery,
                Query = data.InlineQuery.Query.GetInlineQueryAt<string>(1)
            }),
            "search" => await _mediatorService.EnqueueAsync(new AnswerInlineQueryWebSearchBotRequestModel() {
                BotToken = data.Options.Token,
                InlineQuery = data.InlineQuery,
                Query = data.InlineQuery.Query.Replace(inlineCmd, "").Trim()
            }),
            "subdl" => await _mediatorService.EnqueueAsync(new AnswerInlineQuerySubdlRequest() {
                BotToken = data.Options.Token,
                InlineQuery = data.InlineQuery
            }),
            _ => await _mediatorService.EnqueueAsync(new AnswerInlineQueryGuideBotRequestModel() {
                BotToken = data.Options.Token,
                InlineQuery = data.InlineQuery
            })
        };

        _logger.LogInformation("InlineSearchBotApi: {@Result}", result);
    }
}