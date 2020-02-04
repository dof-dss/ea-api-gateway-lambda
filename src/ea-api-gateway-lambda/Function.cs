using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.APIGateway;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Contracts;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using WebPush;

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
            _serviceCollection.AddScoped<IDynamoDBContext, DynamoDBContext>
                (provider =>
                {
                    var amazonDynamoDbClient = new AmazonDynamoDBClient(accessKey, secretKey, RegionEndpoint.EUWest2);
                    return new DynamoDBContext(amazonDynamoDbClient);
                });
            _serviceCollection.AddScoped<WebPushClient, WebPushClient>(provider => CreateWebPushClient());
            _serviceCollection.AddScoped<IApiGatewayManager, ApiGatewayManager>();
            _serviceCollection.AddScoped<IApiGatewayHandlerFactory, ApiGatewayHandlerFactory>();

            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        private WebPushClient CreateWebPushClient()
        {
            var vapidPublicKey = Environment.GetEnvironmentVariable("vapidPublicKey");
            var vapidPrivateKey = Environment.GetEnvironmentVariable("vapidPrivateKey");
            var vapidDetails = new VapidDetails("mailto:example@example.com", vapidPublicKey, vapidPrivateKey);

            var webPushClient = new WebPushClient();
            webPushClient.SetVapidDetails(vapidDetails);

            return webPushClient;
        }

        ~Function()
        {
            var disposable = _serviceProvider as IDisposable;
            disposable?.Dispose();
        }
    }
}
