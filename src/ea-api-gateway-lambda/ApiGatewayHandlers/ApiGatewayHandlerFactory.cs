using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Lambda.APIGatewayEvents;
using Contracts;

namespace ea_api_gateway_lambda
{
    public interface IApiGatewayHandlerFactory
    {
        IApiGatewayHandler Create(APIGatewayProxyRequest request);
    }

    public class ApiGatewayHandlerFactory : IApiGatewayHandlerFactory
    {
        private readonly IApiGatewayManager _apiGatewayManager;

        public ApiGatewayHandlerFactory(IApiGatewayManager apiGatewayManager)
        {
            _apiGatewayManager = apiGatewayManager;
        }

        public IApiGatewayHandler Create(APIGatewayProxyRequest request)
        {
            switch (request.HttpMethod)
            {
                case "OPTIONS":
                    return new OptionsApiGatewayHandler(_apiGatewayManager, request);
                case "GET":
                    return new GetApiGatewayHandler(_apiGatewayManager, request);
                case "POST":
                    return new PostApiGatewayHandler(_apiGatewayManager, request);
                case "PUT":
                    return new PutApiGatewayHandler(_apiGatewayManager, request);
                case "DELETE":
                    return new DeleteApiGatewayHandler(_apiGatewayManager, request);
                default:
                    throw new NotImplementedException($"Http {request.HttpMethod} not implemented ");
            }
        }
    }
}
