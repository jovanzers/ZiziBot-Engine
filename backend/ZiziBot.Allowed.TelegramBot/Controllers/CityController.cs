using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class CityController : CommandController
{
    private readonly MediatorService _mediator;

    public CityController(MediatorService mediator)
    {
        _mediator = mediator;
    }

    [Command("ac")]
    [Command("addcity")]
    [Command("add_city")]
    public async Task AddCity(MessageData data)
    {
        await _mediator.EnqueueAsync(new AddCityRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
            CityName = data.Params,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("city")]
    [Command("lc")]
    public async Task GetCity(MessageData data)
    {
        await _mediator.EnqueueAsync(new GetCityListRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}