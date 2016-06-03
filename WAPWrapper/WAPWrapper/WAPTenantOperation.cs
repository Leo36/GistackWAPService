using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.WindowsAzure.Server.AdminManagement;
using Microsoft.WindowsAzure.Server.Management;
using Microsoft.WindowsAzure.Management.Models;
using WAPWrapper.SPFGallery;
using WAPWrapper.SPFVMM;
using WAPWrapper.SPFVMM.MicrosoftCompute;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Linq;

namespace WAPWrapper
{
    /// <summary>
    /// A disposable class to perform Virtual Machine and other VMM operations (Cloud, User Role) for a User role (Administrator or Tenant or Self-Service User)
    /// </summary>
    public class WAPTenantOperation : IDisposable
    {
        private string TenantUserName { get; set; }
        private string TenantPassword { get; set; }
        private string TenantUserRoleName { get; set; }
        private string TenantSubscriptionID { get; set; }
        private Guid TenantUserRoleID { get; set; }
        private string TenantHeaderValue { get; set; }
        private AuthType TenantAuthType { get; set; }  

        private static string WAPADAuthEndpoint { get; set; }
        private static string WAPASPAuthEndpoint { get; set; }
        private static string WAPTenantEndpoint { get; set; }

        private WAPAuthService WAPAuth { get; set; }
        private WAPTenantService WAPTenant { get; set; }
        public VMMOperation VMMTenant { get; set; }

        /// <summary>
        /// Instantiates VMOperation
        /// </summary>
        public WAPTenantOperation()
        {
            WAPAuth = WAPAuthService.Instance;
            WAPTenant = new WAPTenantService();
        }

        /// <summary>
        /// Sets the WAP Auth Endpoints for issuing admin and tenant tokens
        /// </summary>
        /// <param name="adAuthEndpoint"></param>
        /// <param name="aspAuthEndpoint"></param>
        public void SetWAPAuthContext(string adAuthEndpoint, string aspAuthEndpoint)
        {
            WAPADAuthEndpoint = adAuthEndpoint;
            WAPASPAuthEndpoint = aspAuthEndpoint;

            WAPAuth.SetWAPAuthEndpoint(WAPADAuthEndpoint, WAPASPAuthEndpoint);
        }

        /// <summary>
        /// Sets the WAP Tenent context for Authentication and Tenant User detail
        /// </summary>
        /// <param name="tenantEndpoint"></param>
        /// <param name="authenticationType"></param>
        /// <param name="tenantUserName"></param>
        /// <param name="tenantPassword"></param>
        /// <param name="adfsEndpoint"></param>
        public void SetWAPTenantContext(string tenantEndpoint, AuthType authenticationType, string tenantUserName, string tenantPassword, string adfsEndpoint = "")
        {
            WAPTenantEndpoint = tenantEndpoint;
            TenantUserName = tenantUserName;
            TenantPassword = tenantPassword;
            TenantAuthType = authenticationType;

            switch (authenticationType)
            {
                case AuthType.WindowsAuth:
                    TenantHeaderValue = WAPAuth.PerformADAuth(TenantUserName, TenantPassword);
                    break;
                case AuthType.ASPAuth:
                    TenantHeaderValue = WAPAuth.PerformASPAuth(TenantUserName, TenantPassword);
                    break;
                case AuthType.ADFSAuth:
                    TenantHeaderValue = WAPAuth.PerformADFSAuth(TenantUserName, TenantPassword);
                    break;
                case AuthType.ADASPAuth:
                    TenantHeaderValue = WAPAuth.PerformADFSASPAuth(adfsEndpoint, TenantUserName, TenantPassword);
                    break;
            }
            if (TenantHeaderValue != null)
            {
                WAPTenant.SetWAPEndpoint(WAPTenantEndpoint, TenantUserName, TenantHeaderValue);
                WAPTenant.WAPServiceHttpClient();
            }
            else
            {
                throw new Exception("Failed to set the WAP Tenant Context for user..." + TenantUserName);
            }
        }

        /// <summary>
        /// Gets VMM context for a particular subscription
        /// </summary>
        /// <param name="subscriptionID"></param>
        public void GetVMMContextForSubscription(string subscriptionID)
        {
            VMMTenant = new VMMOperation();
            VMMTenant.GetVMMContextForTenant(WAPTenantEndpoint, TenantUserName, subscriptionID, TenantHeaderValue);
        }

        /// <summary>
        /// Get Tenant user detail
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public User GetTenantUser()
        {
            Uri requestUri = WAPTenant.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, Constants.RelativePaths.User, TenantUserName));
            var user = WAPTenant.GetWAPAsync<User>(requestUri);
            return user.Result;
        }

        /// <summary>
        /// creates wap user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User> CreateUser(string username, string email)
        {
            Uri requestUri = WAPTenant.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, Constants.RelativePaths.Users));
            var userInfo = new User()
            {
                Name = username,
                Email = email,
                State = UserState.Active,
            };
            return await WAPTenant.SendAsync<User, User>(requestUri, new System.Net.Http.HttpMethod(Constants.HttpMethods.Post), userInfo); 
        }

        /// <summary>
        /// updates wap user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User> UpdateUser(string username, string email)
        {
            Uri requestUri = WAPTenant.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, Constants.RelativePaths.User, username));
            var userInfo = new User()
            {
                Name = username,
                Email = email,
            };
            return await WAPTenant.SendAsync<User, User>(requestUri, new System.Net.Http.HttpMethod(Constants.HttpMethods.Put), userInfo); 
        }

        /// <summary>
        /// Gets all available plans
        /// </summary>
        /// <returns></returns>
        public PlanList GetAvailablePlans()
        {
            Uri requestUri = WAPTenant.CreateRequestUri(Constants.RelativePaths.Plans);
            var plans = WAPTenant.GetWAPAsync<PlanList>(requestUri);
            return plans.Result;
        }

        /// <summary>
        /// Gets all tenant subscriptions
        /// </summary>
        /// <returns></returns>
        public AdminSubscriptionList GetSubscriptions()
        {
            Uri requestUri = WAPTenant.CreateRequestUri(Constants.RelativePaths.Subscriptions);
            var subscriptions = WAPTenant.GetWAPAsync<AdminSubscriptionList>(requestUri);
            return subscriptions.Result;
        }

        /// <summary>
        /// Creates subscription for a plan
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="offerCategory"></param>
        /// <returns></returns>
        public async Task<Subscription> CreateSubscription(string planId, string planName)
        {
            Uri requestUri = WAPTenant.CreateRequestUri(Constants.RelativePaths.Subscriptions);

            // Create a subscription for the above user account
            var subscriptionInfo = new AzureProvisioningInfo()
            {
                SubscriptionId = Guid.NewGuid(),
                FriendlyName = planName,
                AccountAdminLiveEmailId = TenantUserName,
                ServiceAdminLiveEmailId = TenantUserName,
                AccountAdminLivePuid = TenantUserName,
                ServiceAdminLivePuid = TenantUserName,
                OfferCategory = planName,
                PlanId = planId
            };
            return await WAPTenant.SendAsync<AzureProvisioningInfo, Subscription>(requestUri, new HttpMethod(Constants.HttpMethods.Post), subscriptionInfo); 
        }

        /// <summary>
        /// Updates the specified subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="friendlyName">The friendly name.</param>
        /// <returns>Async task.</returns>
        public async Task<Subscription> UpdateSubscriptionNameAsync(string subscriptionId, string subscriptionName)
        {
            var subscription = new Subscription
            {
                SubscriptionID = subscriptionId,
                SubscriptionName = subscriptionName,
                CoAdminNames = null, // null means won't change for PATCH
                State = SubscriptionState.Active
            };

            Uri requestUri = WAPTenant.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, Constants.RelativePaths.Subscription, subscriptionId));
            return await WAPTenant.SendAsync<Subscription, Subscription>(requestUri, new HttpMethod(Constants.HttpMethods.Patch), subscription);
        }


        /// <summary>
        /// Gets usage summary data for a tenant subscriptions
        /// </summary>
        /// <returns></returns>
        public UsageSummaryList GetTenantQuotaForSubscription(string subscriptionId)
        {
            Uri requestUri = WAPTenant.CreateRequestUri(String.Format(CultureInfo.InvariantCulture, 
                                                                    Constants.RelativePaths.SubscriptionUsageSummaries, subscriptionId));
            var usageSummary = WAPTenant.GetWAPAsync<UsageSummaryList>(requestUri);
            return usageSummary.Result;
        }

        /// <summary>
        /// Gets Cloud Service
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="cloudServiceName"></param>
        /// <returns></returns>
        public CloudService GetCloudService(string subscriptionId, string cloudServiceName)
        {
            Uri requestUri = WAPTenant.CreateRequestUri(String.Format(CultureInfo.InvariantCulture,
                                                                      Constants.ServiceProviderFoundation.SubCloudService, subscriptionId, cloudServiceName),
                                                                      //"{0}/{1}/{2}", subscriptionId, Constants.ServiceProviderFoundation.CloudServices, 
                                                                      //cloudServiceName),
                                                                      new KeyValuePair<string, string>("api-version", "2013-03"));

            var response = WAPTenant.SendAsyncString(requestUri, new HttpMethod(Constants.HttpMethods.Get));
            var result = Parser.FromJSONString<CloudService>(response.Result).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Creates cloud service for a subscription
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="offerCategory"></param>
        /// <returns></returns>
        public async Task<CloudService> CreateCloudService(string subscriptionId, CloudService cloudService)
        {
            Uri requestUri = WAPTenant.CreateRequestUri(String.Format(CultureInfo.InvariantCulture,
                                                                      Constants.ServiceProviderFoundation.SubCloudServices, subscriptionId),
                                                                      //"{0}/{1}", subscriptionId, Constants.ServiceProviderFoundation.CloudServices),
                                                                      new KeyValuePair<string, string>("api-version", "2013-03"));

            String jsonString = Parser.ToJSON(cloudService);

            var response = await WAPTenant.SendAsyncString(requestUri, new HttpMethod(Constants.HttpMethods.Post), jsonString, TenantUserName, true);
            return Parser.FromJSONString<CloudService>(response).FirstOrDefault();
        }

        /// <summary>
        /// Gets all gallery items for a subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public IEnumerable<VMRoleGalleryItem> GetVMRoleGalleryItems(string subscriptionId)
        {
            Uri requestUri = WAPTenant.CreateRequestUri(String.Format(CultureInfo.InvariantCulture,
                                                        Constants.ServiceProviderFoundation.SubVMRoleGalleryItems, subscriptionId),
                                                        //"{0}/{1}", subscriptionId, Constants.ServiceProviderFoundation.GalleryItems),
                                                        new KeyValuePair<string, string>("api-version", "2013-03"));

            var response = WAPTenant.SendAsyncString(requestUri, new HttpMethod(Constants.HttpMethods.Get));
            return Parser.FromJSONString<VMRoleGalleryItem>(response.Result);
        }

        /// <summary>
        /// Gets all resource definitions for a subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public VMRoleResourceDefinition GetCloudServiceResourceDefinition(string subscriptionId, string relativePath)
        {
            Uri requestUri = WAPTenant.CreateRequestUri(String.Format(CultureInfo.InvariantCulture,
                                                        "{0}/{1}", subscriptionId, relativePath),
                                                        new KeyValuePair<string, string>("api-version", "2013-03"));

            var response = WAPTenant.SendAsyncString(requestUri, new HttpMethod(Constants.HttpMethods.Get));
            return Parser.FromJSONString<VMRoleResourceDefinition>(response.Result).FirstOrDefault();
        }

        /// <summary>
        /// Creates VM Role for a subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="cloudServiceName"></param>
        /// <param name="vmRole"></param>
        /// <returns></returns>
        public async Task<VMRole> CreateVMRole(string subscriptionId, string cloudServiceName, VMRole vmRole)
        {
            Uri requestUri = WAPTenant.CreateRequestUri(String.Format(CultureInfo.InvariantCulture,
                                                                        Constants.ServiceProviderFoundation.SubCloudServiceVMRoles,
                                                                        subscriptionId, cloudServiceName),
                                                                        //"{0}/{1}/{2}/{3}", subscriptionId, Constants.ServiceProviderFoundation.CloudServices, 
                                                                        //cloudServiceName, Constants.ServiceProviderFoundation.VMRoles),
                                                                        new KeyValuePair<string, string>("api-version", "2013-03"));

            String jsonString = Parser.ToJSON(vmRole);

            var response = await WAPTenant.SendAsyncString(requestUri, new HttpMethod(Constants.HttpMethods.Post), jsonString, TenantUserName, true);
            return Parser.FromJSONString<VMRole>(response).FirstOrDefault();
        }

        private bool disposed = false;

        /// <summary>
        /// Dispose the object for system/users call
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //WAPAuth = null;
                    WAPTenant = null;
                    VMMTenant.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Dispose the object per users call
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
