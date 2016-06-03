using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Server.AdminManagement;
using Microsoft.WindowsAzure.Server.Management;
using Microsoft.WindowsAzure.Management.Models;

namespace WAPWrapper
{
    /// <summary>
    /// A class to instantiate WAP Tenant context to perform WAP Tenant functions
    /// </summary>
    public class WAPTenantService : WAPBaseService
    {
        private string TenantUserName { get; set; }
        private string TenantUser { get; set; }
        private string TenantDomain { get; set; }
        private string TenantPassword { get; set; }
        private string TenantUserRoleName { get; set; }
        private string TenantSubscriptionID { get; set; }
        private string TenantHeaderValue { get; set; }
        private AuthType TenantAuthType { get; set; }
        private string TenantEndpoint { get; set; }

        //private static string mediaType = "application/json"; // application/json or application/xml

        /// <summary>
        /// Initializes a new instance of the ManagementClient class.
        /// </summary>
        /// <param name="baseEndpoint">API endpoint</param>
        /// <param name="token">The token.</param>
        public WAPTenantService()
            :base()
        {
        }

        /// <summary>
        /// Sets WAP Tenant Context for Endpoint and Tenant user credentials
        /// </summary>
        /// <param name="tenantEndpoint"></param>
        /// <param name="tenantUserName"></param>
        /// <param name="tenantHeaderValue"></param>
        public void SetWAPEndpoint(string tenantEndpoint, string tenantUserName, string tenantHeaderValue)
        {
            TenantEndpoint = tenantEndpoint;
            TenantUserName = tenantUserName;
            TenantHeaderValue = tenantHeaderValue;
        }

        /// <summary>
        /// Sets tenant user role detail based on the subscription ID
        /// </summary>
        /// <param name="subscriptionID"></param>
        public void SetTenantSubscription(string subscriptionID)
        {
            this.TenantSubscriptionID = subscriptionID;
            switch (TenantAuthType)
            {
                case AuthType.WindowsAuth:
                    this.TenantUserRoleName = subscriptionID;
                    break;
                case AuthType.ASPAuth:
                    this.TenantUserRoleName = string.Format("{0}_{1}", TenantUserName, TenantSubscriptionID);
                    break;
                default:
                    this.TenantUserRoleName = subscriptionID;
                    break;
            }
        }

        /// <summary>
        /// Method to set the httpClient for WAP Tenant operations
        /// </summary>
        public void WAPServiceHttpClient(bool acceptJSON = true)
        {
            httpClient = new HttpClient();
            if (acceptJSON)
            {
                mediaType = JSONMediaType;
            }
            else
            {
                mediaType = XMLMediaType;
            }
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            string WAPURL = string.Format("{0}/", TenantEndpoint);
            httpClient.BaseAddress = new UriBuilder(WAPURL).Uri;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TenantHeaderValue);
            httpClient.DefaultRequestHeaders.Add("x-ms-principal-id", TenantUserName);
            //var acceptHeader = new MediaTypeWithQualityHeaderValue(mediaType);
            //acceptHeader.Parameters.Add(new NameValueHeaderValue("odata", "verbose"));
            //httpClient.DefaultRequestHeaders.Accept.Add(acceptHeader);
        }
    }
}