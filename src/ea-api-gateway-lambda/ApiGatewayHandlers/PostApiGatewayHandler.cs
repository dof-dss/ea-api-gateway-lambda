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
        }
        public override async Task<APIGatewayProxyResponse> Execute()
        {
            return GetAPIGatewayResponse(HttpStatusCode.OK, string.Empty);
        }
    }
}