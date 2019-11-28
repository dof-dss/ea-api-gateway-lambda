using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Common.Models;
using Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ea_api_gateway_lambda
{
    public class App
    {
        private readonly IApiGatewayManager _apiGatewayManager;
        public JsonSerializerSettings JsonSettings { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public App(IApiGatewayManager apiGatewayManager)
        {
            Headers = new Dictionary<string, string> {
                {"Access-Control-Allow-Origin", "*"},
                {"Access-Control-Allow-Headers", "*"},
                {"Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS"}
            };
            JsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            _apiGatewayManager = apiGatewayManager;
        }

        public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest request)
        {
            // Determine which method to execute
            switch (request.HttpMethod)
            {
                case "OPTIONS":
                    return GetAPIGatewayResponse(HttpStatusCode.OK, string.Empty);
                case "GET":
                    switch (request.Resource)
                    {
                        case "/apis":
                            return GetAPIGatewayResponse(HttpStatusCode.OK, await _apiGatewayManager.GetAllApis());
                        case "/usageplans":
                            return GetAPIGatewayResponse(HttpStatusCode.OK, await _apiGatewayManager.GetUsagePlans());
                        case "/swagger":
                            return GetAPIGatewayResponse(HttpStatusCode.OK, 
                                await _apiGatewayManager.GetOpenApi(request.QueryStringParameters["apiId"], 
                                    request.QueryStringParameters["stage"], "swagger"));
                        case "/documentation":
                            return GetAPIGatewayResponse(HttpStatusCode.OK,
                                await _apiGatewayManager.GetApiDocumentation(request.QueryStringParameters["apiId"]));
                        case "/sdk":
                            return GetAPIGatewayResponse(HttpStatusCode.OK,
                                await _apiGatewayManager.GetSdk(request.QueryStringParameters["apiId"],
                                    request.QueryStringParameters["stage"], request.QueryStringParameters["sdkType"]));
                        default:
                            throw new NotImplementedException($"Http {request.Resource} not implemented ");
                    }
                case "POST":
                    return GetAPIGatewayResponse(HttpStatusCode.Created, string.Empty);
                case "PUT":
                    switch (request.Resource)
                    {
                        case "/subscribe":
                            var subscriptionModel = JsonConvert.DeserializeObject<SubscriptionModel>(request.Body);
                            return GetAPIGatewayResponse(HttpStatusCode.Created,
                                await _apiGatewayManager.Subscribe(subscriptionModel));
                        default:
                            throw new NotImplementedException($"Http {request.Resource} not implemented ");
                    }
                case "DELETE":
                    return GetAPIGatewayResponse(HttpStatusCode.NoContent, string.Empty);
                default:
                    throw new NotImplementedException($"Http {request.HttpMethod} not implemented ");
            }
        }


        private APIGatewayProxyResponse GetAPIGatewayResponse(HttpStatusCode statusCode, object responseContent)
        {
            return new APIGatewayProxyResponse
            {
                Headers = this.Headers,
                StatusCode = (int)statusCode,
                Body = JsonConvert.SerializeObject(responseContent, JsonSettings)
            };
        }
    }
}
