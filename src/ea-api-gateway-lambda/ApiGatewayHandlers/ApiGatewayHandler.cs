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
    public interface IApiGatewayHandler
    {
        Task<APIGatewayProxyResponse> Execute();
    }

    public abstract class ApiGatewayHandler : IApiGatewayHandler
    {
        protected IApiGatewayManager ApiGatewayManager;
        protected JsonSerializerSettings JsonSettings { get; set; }
        protected Dictionary<string, string> Headers { get; set; }
        protected APIGatewayProxyRequest Request { get; set; }
        protected IDictionary<string, Func<Task<APIGatewayProxyResponse>>> GatewayFunctionMapper;

        public virtual async Task<APIGatewayProxyResponse> Execute()
        {
            return GatewayFunctionMapper.ContainsKey(Request.Resource) ?
                await GatewayFunctionMapper[Request.Resource]()
                : throw new NotImplementedException($"Http {Request.Resource} not implemented ");
        }

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
            GatewayFunctionMapper = new Dictionary<string, Func<Task<APIGatewayProxyResponse>>>();
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
    }
}