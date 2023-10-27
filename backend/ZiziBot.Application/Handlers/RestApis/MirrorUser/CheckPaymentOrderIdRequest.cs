using System.Net;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class CheckPaymentOrderIdRequest : ApiRequestBase<TrakteerParsedDto>
{
    public string OrderId { get; set; }
}

public class CheckPaymentOrderIdHandler : IApiRequestHandler<CheckPaymentOrderIdRequest, TrakteerParsedDto>
{
    public async Task<ApiResponseBase<TrakteerParsedDto>> Handle(CheckPaymentOrderIdRequest request, CancellationToken cancellationToken)
    {
        var parsedTrakteer = await request.OrderId.GetTrakteerUrl().ParseTrakteerWeb();

        if (!parsedTrakteer.IsValid)
        {
            return new ApiResponseBase<TrakteerParsedDto>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "OrderId not found"
            };
        }

        return new ApiResponseBase<TrakteerParsedDto>()
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Get OrderId succeed",
            Result = parsedTrakteer
        };
    }
}