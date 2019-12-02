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

        public App(IApiGatewayManager apiGatewayManager)
        {
            _apiGatewayManager = apiGatewayManager;
        }

        public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest request)
        {
            return await ApiGatewayResponse.Create(request, _apiGatewayManager).Execute();
        }
    }
}
