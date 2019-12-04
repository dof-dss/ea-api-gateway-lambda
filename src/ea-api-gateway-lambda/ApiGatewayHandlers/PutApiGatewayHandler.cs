using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Common.Models;
using Contracts;
using Newtonsoft.Json;

namespace ea_api_gateway_lambda
{
    public class PutApiGatewayHandler : ApiGatewayHandler
    {
        public PutApiGatewayHandler(IApiGatewayManager apiGatewayManager, APIGatewayProxyRequest request) : base(apiGatewayManager, request)
        {
            GatewayFunctionMapper.Add("/subscribe", Subscribe);
        }

        private async Task<APIGatewayProxyResponse> Subscribe()
        {
            var subscriptionModel = JsonConvert.DeserializeObject<SubscriptionModel>(Request.Body);
            return GetAPIGatewayResponse(HttpStatusCode.Created,
                await ApiGatewayManager.Subscribe(subscriptionModel));
        }

    }
}