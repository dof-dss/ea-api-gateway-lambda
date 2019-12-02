using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Contracts;

namespace ea_api_gateway_lambda
{
    public class GetApiGatewayResponse : ApiGatewayResponse
    {
        public GetApiGatewayResponse(IApiGatewayManager apiGatewayManager, APIGatewayProxyRequest request) : base(apiGatewayManager, request)
        {
        }

        public override async Task<APIGatewayProxyResponse> Execute()
        {
            switch (Request.Resource)
            {
                case "/apis":
                    return GetAPIGatewayResponse(HttpStatusCode.OK, await ApiGatewayManager.GetAllApis());
                case "/usageplans":
                    return GetAPIGatewayResponse(HttpStatusCode.OK, await ApiGatewayManager.GetUsagePlans());
                case "/swagger":
                    return GetAPIGatewayResponse(HttpStatusCode.OK,
                        await ApiGatewayManager.GetOpenApi(Request.QueryStringParameters["apiId"],
                            Request.QueryStringParameters["stage"], "swagger"));
                case "/documentation":
                    return GetAPIGatewayResponse(HttpStatusCode.OK,
                        await ApiGatewayManager.GetApiDocumentation(Request.QueryStringParameters["apiId"]));
                case "/sdk":
                    return GetAPIGatewayResponse(HttpStatusCode.OK,
                        await ApiGatewayManager.GetSdk(Request.QueryStringParameters["apiId"],
                            Request.QueryStringParameters["stage"], Request.QueryStringParameters["sdkType"]));
                default:
                    throw new NotImplementedException($"Http {Request.Resource} not implemented ");
            }
        }
    }
}