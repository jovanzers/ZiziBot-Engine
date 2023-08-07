using System.Reflection;
using MediatR;
using MediatR.Extensions.AttributedBehaviors;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using ZiziBot.Application.Handlers.Telegram.Middleware;

namespace ZiziBot.Infrastructure;

public static class MediatRExtension
{
    internal static IServiceCollection AddMediator(this IServiceCollection services)
    {
        var assembly = typeof(PingRequestHandler).GetTypeInfo().Assembly;

        services.AddMediatR(
            configuration => configuration
                .RegisterServicesFromAssemblyContaining<PingRequestHandler>()
                .AddOpenBehavior(typeof(BotMiddlewarePipelineBehaviour<,>))
                .AddOpenRequestPreProcessor(typeof(ChatRestrictionProcessorBotRequest<>))
                .AddOpenRequestPostProcessor(typeof(CheckAfkSessionBehavior<,>))
                .AddOpenRequestPostProcessor(typeof(EnsureChatAdminRequestHandler<,>))
                .AddOpenRequestPostProcessor(typeof(EnsureChatSettingBehavior<,>))
                .AddOpenRequestPostProcessor(typeof(FindNoteRequestHandler<,>))
                .AddOpenRequestPostProcessor(typeof(UpsertBotUserHandler<,>))
        );

        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(GlobalExceptionHandler<,,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));

        services.AddMediatRAttributedBehaviors(assembly);

        return services;
    }
}