using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Common.Models;
using Contracts;
using Newtonsoft.Json;

namespace ea_api_gateway_lambda
{
    public class PostApiGatewayHandler : ApiGatewayHandler
    {
        public PostApiGatewayHandler(IApiGatewayManager apiGatewayManager, APIGatewayProxyRequest request) : base(apiGatewayManager, request)
        {
            GatewayFunctionMapper.Add("/push/subscribe", SubscribePush);
            GatewayFunctionMapper.Add("/push", Push);
        }

        private async Task<APIGatewayProxyResponse> Push()
        {
            return GetAPIGatewayResponse(HttpStatusCode.Created,
                await ApiGatewayManager.SendPush(Request.Body));
        }

        private async Task<APIGatewayProxyResponse> SubscribePush()
        {
            var pushSubscriptionModel = JsonConvert.DeserializeObject<PushSubscriptionModel>(Request.Body);
            return GetAPIGatewayResponse(HttpStatusCode.Created,
                await ApiGatewayManager.PushSubscribe(pushSubscriptionModel));
        }
    }
}