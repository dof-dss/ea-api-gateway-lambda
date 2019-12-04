using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.APIGateway.Model;
using Common.Models;

namespace Contracts
{
    public interface IApiGatewayManager
    {
        Task<IEnumerable<ApiOverviewModel>> GetAllApis();
        Task<object> GetOpenApi(string apiId, string stage, string exportType);
        Task<object> GetUsagePlans();
        Task<object> GetUsage(string keyId, string usagePlanId);
        Task<string> Subscribe(SubscriptionModel subscriptionModel);
        Task<DocumentModel> GetApiDocumentation(string apiId);
        Task<SDKModel> GetSdk(string apiId, string stage, string sdkType);
        Task<IEnumerable<ApiKeyModel>> GetApiKeys(string identityId);
    }
}