using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Contracts;

namespace ea_api_gateway_lambda
{
    public class GetApiGatewayHandler : ApiGatewayHandler
    {
        public GetApiGatewayHandler(IApiGatewayManager apiGatewayManager, APIGatewayProxyRequest request) : base(apiGatewayManager, request)
        {
            GatewayFunctionMapper.Add("/apis", GetAllApis);
            GatewayFunctionMapper.Add("/usageplans", GetUsagePlans);
            GatewayFunctionMapper.Add("/swagger", GetSwagger);
            GatewayFunctionMapper.Add("/documentation", GetDocumentation);
            GatewayFunctionMapper.Add("/sdk", GetSdk);
            GatewayFunctionMapper.Add("/apikeys", GetApiKeys);
            GatewayFunctionMapper.Add("/usage", GetUsage);
        }

        private async Task<APIGatewayProxyResponse> GetAllApis() => 
            GetAPIGatewayResponse(HttpStatusCode.OK, await ApiGatewayManager.GetAllApis());

        private async Task<APIGatewayProxyResponse> GetUsagePlans() =>
            GetAPIGatewayResponse(HttpStatusCode.OK, await ApiGatewayManager.GetUsagePlans());

        private async Task<APIGatewayProxyResponse> GetSwagger() =>
            GetAPIGatewayResponse(HttpStatusCode.OK,
                await ApiGatewayManager.GetOpenApi(Request.QueryStringParameters["apiId"],
                    Request.QueryStringParameters["stage"], "swagger"));

        private async Task<APIGatewayProxyResponse> GetDocumentation() =>
            GetAPIGatewayResponse(HttpStatusCode.OK,
                await ApiGatewayManager.GetApiDocumentation(Request.QueryStringParameters["apiId"]));

        private async Task<APIGatewayProxyResponse> GetSdk() =>
            GetAPIGatewayResponse(HttpStatusCode.OK,
                await ApiGatewayManager.GetSdk(Request.QueryStringParameters["apiId"],
                    Request.QueryStringParameters["stage"], Request.QueryStringParameters["sdkType"]));

        private async Task<APIGatewayProxyResponse> GetApiKeys() =>
            GetAPIGatewayResponse(HttpStatusCode.OK,
                await ApiGatewayManager.GetApiKeys(Request.QueryStringParameters["identityId"]));

        private async Task<APIGatewayProxyResponse> GetUsage() =>
            GetAPIGatewayResponse(HttpStatusCode.OK,
                await ApiGatewayManager.GetUsage(Request.QueryStringParameters["keyId"], 
                    Request.QueryStringParameters["usagePlanId"]));
    }
}