using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.APIGateway;
using Amazon.APIGateway.Model;
using Common;
using Common.Models;
using Contracts;
using Newtonsoft.Json;

namespace Domain
{
    public class ApiGatewayManager: IApiGatewayManager
    {
        private readonly IAmazonAPIGateway _amazonApiGatewayClient;

        public ApiGatewayManager(IAmazonAPIGateway amazonApiGateway) =>
            _amazonApiGatewayClient = amazonApiGateway;

        public async Task<IEnumerable<ApiOverviewModel>>  GetAllApis()
        {
            GetRestApisResponse result = await _amazonApiGatewayClient.GetRestApisAsync(new GetRestApisRequest()
            {
                Limit = 100
            }, CancellationToken.None);

            return result.Items.Where(i => i.Tags.ContainsKey("demo")).Select(x => new ApiOverviewModel
            {
                Id = x.Id, 
                Name = x.Name,
                Description = GetDescription(x.Id)
            });
        }

        private string GetDescription(string apiId)
        {
            var result = _amazonApiGatewayClient.GetDocumentationPartsAsync(new GetDocumentationPartsRequest
            {
                Limit = 100,
                RestApiId = apiId,
                Type = DocumentationPartType.API
            }).Result;

            var properties = result?.Items?.FirstOrDefault()?.Properties;
            return properties != null ? JsonConvert.DeserializeObject<DocumentModel>(properties).Description : null;
        }

        public async Task<DocumentModel> GetApiDocumentation(string apiId)
        {
            var result = await _amazonApiGatewayClient.GetDocumentationPartsAsync(new GetDocumentationPartsRequest
            {
                Limit = 100,
                RestApiId = apiId,
                Type = DocumentationPartType.API
            });

            var properties = result?.Items?.FirstOrDefault()?.Properties;
            return properties != null ? JsonConvert.DeserializeObject<DocumentModel>(properties) : null;
        }

        public async Task<object> GetUsagePlans()
        {
            GetUsagePlansResponse result = await _amazonApiGatewayClient.GetUsagePlansAsync(new GetUsagePlansRequest()
            {
                Limit = 100
            }, CancellationToken.None);

            return result.Items;
        }

        public async Task<object> GetUsage(string keyId, string usagePlanId)
        {
            var result = await _amazonApiGatewayClient.GetUsageAsync(new GetUsageRequest()
            {
                KeyId = keyId,
                UsagePlanId = usagePlanId,
                StartDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"),
                EndDate = DateTime.Now.ToString( "yyyy-MM-dd")
            }, CancellationToken.None);

            return result.Items;
        }

        public async Task<object> GetOpenApi(string apiId, string stage, string exportType)
        {
            var result = await _amazonApiGatewayClient.GetExportAsync(new GetExportRequest
            {
                RestApiId = apiId,
                StageName = stage,
                ExportType = exportType,
                Accepts = "application/json"
            });

            return result.Body.DeserializeFromStream();
        }

        public async Task<SDKModel> GetSdk(string apiId, string stage, string sdkType)
        {
            var result = await _amazonApiGatewayClient.GetSdkAsync(new GetSdkRequest
            {
                RestApiId = apiId,
                StageName = stage,
                SdkType = sdkType
            });

            return new SDKModel
            {
                Body = result.Body,
                ContentType = result.ContentType,
                ContentDisposition = result.ContentDisposition
            };
        }

        public async Task<string> Subscribe(SubscriptionModel subscriptionModel)
        {
            var getApiKeysResponse = await GetApiKeysRequest(subscriptionModel.IdentityId);

            if (getApiKeysResponse.Items.Any())
                return getApiKeysResponse.Items.First().Value;

            var createApiKeyResponse = await CreateApiKeyRequest(subscriptionModel.IdentityId);
            await CreateUsagePlanKey(createApiKeyResponse.Id, subscriptionModel.UsagePlanId);
            return createApiKeyResponse.Value;
        }

        public async Task<IEnumerable<ApiKeyModel>> GetApiKeys(string identityId)
        {
            var getApiKeysResponse = await GetApiKeysRequest(identityId);

            return getApiKeysResponse.Items.Any()
                ? getApiKeysResponse.Items.Select(k => new ApiKeyModel
                {
                    Id = k.Id,
                    Value = k.Value,
                    CustomerId = k.CustomerId,
                    CreatedDate = k.CreatedDate,
                    LastUpdatedDate = k.LastUpdatedDate,
                    Enabled = k.Enabled,
                    Description = k.Description
                })
                : new List<ApiKeyModel>();
        }

        private Task<GetApiKeysResponse> GetApiKeysRequest(string identityId)
        {
            return _amazonApiGatewayClient.GetApiKeysAsync(new GetApiKeysRequest
            {
                Limit = 1,
                IncludeValues = true,
                NameQuery = identityId
            });
        }

        private async Task<CreateApiKeyResponse> CreateApiKeyRequest(string identityId)
        {
            return await _amazonApiGatewayClient.CreateApiKeyAsync(new CreateApiKeyRequest
            {
                Name = identityId,
                Description = $"Dev Portal API Key for Identity Pool user {identityId}",
                Enabled = true,
                GenerateDistinctId = true
            });
        }

        private async Task<CreateUsagePlanKeyResponse> CreateUsagePlanKey(string keyId, string usagePlanId)
        {
            return await _amazonApiGatewayClient.CreateUsagePlanKeyAsync(new CreateUsagePlanKeyRequest
            {
                KeyId = keyId,
                KeyType = "API_KEY",
                UsagePlanId = usagePlanId
            });
        }

        private async Task<GetUsagePlanKeyResponse> GetUsagePlanKey(string keyId, string usagePlanId)
        {
            return await _amazonApiGatewayClient.GetUsagePlanKeyAsync(new GetUsagePlanKeyRequest
            {
                KeyId = keyId,
                UsagePlanId = usagePlanId
            });
        }
    }
}
