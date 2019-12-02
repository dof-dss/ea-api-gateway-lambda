using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ea_api_gateway_lambda
{
    public abstract class ApiGatewayHandler
    {
        protected IApiGatewayManager ApiGatewayManager;
        protected JsonSerializerSettings JsonSettings { get; set; }
        protected Dictionary<string, string> Headers { get; set; }
        protected APIGatewayProxyRequest Request { get; set; }

        public abstract Task<APIGatewayProxyResponse> Execute();

        protected ApiGatewayHandler(IApiGatewayManager apiGatewayManager, APIGatewayProxyRequest request)
        {
            ApiGatewayManager = apiGatewayManager;
            Headers = new Dictionary<string, string>
            {
                {"Access-Control-Allow-Origin", "*"},
                {"Access-Control-Allow-Headers", "*"},
                {"Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS"}
            };
            JsonSettings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            Request = request;
        }

        protected APIGatewayProxyResponse GetAPIGatewayResponse(HttpStatusCode statusCode, object responseContent)
        {
            return new APIGatewayProxyResponse
            {
                Headers = this.Headers,
                StatusCode = (int)statusCode,
                Body = JsonConvert.SerializeObject(responseContent, JsonSettings)
            };
        }

        public static ApiGatewayHandler Create(APIGatewayProxyRequest request, IApiGatewayManager apiGatewayManager)
        {
            switch (request.HttpMethod)
            {
                case "OPTIONS":
                    return new OptionsApiGatewayHandler(apiGatewayManager, request);
                case "GET":
                    return new GetApiGatewayHandler(apiGatewayManager, request);
                case "POST":
                    return new PostApiGatewayHandler(apiGatewayManager, request);
                case "PUT":
                    return new PutApiGatewayHandler(apiGatewayManager, request);
                case "DELETE":
                    return new DeleteApiGatewayHandler(apiGatewayManager, request);
                default:
                    throw new NotImplementedException($"Http {request.HttpMethod} not implemented ");
            }
        }
    }
}