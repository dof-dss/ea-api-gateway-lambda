using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Common.Models;
using Contracts;
using Newtonsoft.Json;

namespace ea_api_gateway_lambda
{
    public class PutApiGatewayResponse : ApiGatewayResponse
    {
        public PutApiGatewayResponse(IApiGatewayManager apiGatewayManager, APIGatewayProxyRequest request) : base(apiGatewayManager, request)
        {
        }

        public override async Task<APIGatewayProxyResponse> Execute()
        {
            switch (Request.Resource)
            {
                case "/subscribe":
                    var subscriptionModel = JsonConvert.DeserializeObject<SubscriptionModel>(Request.Body);
                    return GetAPIGatewayResponse(HttpStatusCode.Created,
                        await ApiGatewayManager.Subscribe(subscriptionModel));
                default:
                    throw new NotImplementedException($"Http {Request.Resource} not implemented ");
            }
        }
    }
}