using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon;
using Amazon.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Contracts;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ea_api_gateway_lambda
{
    public class Function
    {
        private ServiceCollection _serviceCollection;
        private ServiceProvider _serviceProvider;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function() => ConfigureServices();


        /// <summary>
        /// This method is called for every Lambda invocation.
        /// </summary>
        /// <param name="apiGatewayProxyRequest"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apiGatewayProxyRequest, ILambdaContext context) 
            => await _serviceProvider.GetService<App>().Run(apiGatewayProxyRequest);

        private void ConfigureServices()
        {
            var accessKey = Environment.GetEnvironmentVariable("accessKey");
            var secretKey = Environment.GetEnvironmentVariable("secretKey");

            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddTransient<App>();
            _serviceCollection.AddScoped<IAmazonAPIGateway, AmazonAPIGatewayClient>
                (provider => new AmazonAPIGatewayClient(accessKey, secretKey));
            _serviceCollection.AddScoped<IApiGatewayManager, ApiGatewayManager>();

            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        ~Function()
        {
            var disposable = _serviceProvider as IDisposable;
            disposable?.Dispose();
        }
    }
}
