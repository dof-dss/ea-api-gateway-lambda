using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.APIGateway.Model;
using Common.Models;

namespace Contracts
{
    public interface IApiGatewayManager
    {
        Task<object> GetAllApis();
        Task<object> GetOpenApi(string apiId, string stage, string exportType);
        Task<object> GetUsagePlans();
        Task<string> Subscribe(SubscriptionModel subscriptionModel);
        Task<object> GetApiDocumentation(string apiId);
        Task<SDKModel> GetSdk(string apiId, string stage, string sdkType);
    }
}