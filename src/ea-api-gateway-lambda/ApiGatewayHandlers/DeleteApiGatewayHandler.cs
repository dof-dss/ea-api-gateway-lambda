using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Contracts;

namespace ea_api_gateway_lambda
{
    public class DeleteApiGatewayHandler : ApiGatewayHandler
    {
        public DeleteApiGatewayHandler(IApiGatewayManager apiGatewayManager, APIGatewayProxyRequest request) : base(apiGatewayManager, request)
        {
        }

        public override async Task<APIGatewayProxyResponse> Execute()
        {
            return GetAPIGatewayResponse(HttpStatusCode.NoContent, string.Empty);
        }
    }
}