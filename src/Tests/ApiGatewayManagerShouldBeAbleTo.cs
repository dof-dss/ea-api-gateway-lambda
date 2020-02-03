using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.APIGateway;
using Amazon.APIGateway.Model;
using Amazon.DynamoDBv2.DataModel;
using Contracts;
using Domain;
using Moq;
using NUnit.Framework;
using WebPush;

namespace Tests
{
    [TestFixture]
    public class ApiGatewayManagerShouldBeAbleTo
    {
        private IApiGatewayManager _apiGatewayManager;
        private Mock<IAmazonAPIGateway> _amazonApiGatewayMock;
        private Mock<IDynamoDBContext> _amazonDynamoDbContextMock;
        private Mock<WebPushClient> _webPushClient;

        [SetUp]
        public void SetUp()
        {
            _amazonApiGatewayMock = new Mock<IAmazonAPIGateway>();
            _amazonDynamoDbContextMock = new Mock<IDynamoDBContext>();
            _webPushClient = new Mock<WebPushClient>();

            _apiGatewayManager = new ApiGatewayManager(_amazonApiGatewayMock.Object, _amazonDynamoDbContextMock.Object, _webPushClient.Object);
        }

        [Test]
        public async Task GetAllApis()
        {
            var apis = new List<RestApi> {new RestApi {Id = "abcdefghijkl", Name = "TestName"}};
            var docPart = new List<DocumentationPart> {new DocumentationPart {Properties = "{\"description\": \"Test description\"}" } };

            _amazonApiGatewayMock
                .Setup(gw => gw.GetDocumentationPartsAsync(It.IsAny<GetDocumentationPartsRequest>(), new CancellationToken()))
                .ReturnsAsync(new GetDocumentationPartsResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Items = docPart
                });

            _amazonApiGatewayMock
                .Setup(gw => gw.GetRestApisAsync(It.IsAny<GetRestApisRequest>(), new CancellationToken()))
                .ReturnsAsync(new GetRestApisResponse()
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Items = apis
                });

            var results = await _apiGatewayManager.GetAllApis();

            var api = results.ToList().First();
            Assert.AreEqual("abcdefghijkl", api.Id);
            Assert.AreEqual("TestName", api.Name);
            Assert.AreEqual("Test description", api.Description);
        }
    }
}
