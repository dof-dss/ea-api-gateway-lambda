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
        private readonly IApiGatewayHandlerFactory _apiGatewayHandlerFactory;

        public App(IApiGatewayHandlerFactory apiGatewayHandlerFactory) 
            => _apiGatewayHandlerFactory = apiGatewayHandlerFactory;

        public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest request) =>
            await _apiGatewayHandlerFactory.Create(request).Execute();

    }
}
