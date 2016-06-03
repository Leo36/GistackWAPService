using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.WindowsAzure.Server.AdminManagement;
using Microsoft.WindowsAzure.Server.Management;
using Microsoft.WindowsAzure.Management.Models;

namespace WAPWrapper
{
    /// <summary>
    /// A disposable class to perform Virtual Machine and other VMM operations (Cloud, User Role) for a User role (Administrator or Tenant or Self-Service User)
    /// </summary>
    public class WAPAdminOperation : IDisposable
    {

        private static string WAPADAuthEndpoint { get; set; }
        private static string WAPASPAuthEndpoint { get; set; }
        private static string WAPAdminEndpoint { get; set; }
        private static string WAPAdminUserName { get; set; }
        private static string WAPAdminPassword { get; set; }
        private static string WAPAdminHeaderValue { get; set; }

        private WAPAuthService WAPAuth { get; set; }
        private WAPAdminService WAPAdmin { get; set; }
        public VMMOperation VMMAdmin { get; set; }

        /// <summary>
        /// Instantiates VMOperation
        /// </summary>
        public WAPAdminOperation()
        {
            WAPAuth = WAPAuthService.Instance;
            WAPAdmin = new WAPAdminService();
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
        /// Sets the WAP Admin context for Authentication endpoints
        /// </summary>
        /// <param name="adAuthEndpoint"></param>
        /// <param name="aspAuthEndpoint"></param>
        /// <param name="adminEndpoint"></param>
        /// <param name="adminUserName"></param>
        /// <param name="adminPassword"></param>
        public void SetWAPAdminContext(string adminEndpoint, string adminUserName, string adminPassword)
        {
            WAPAdminEndpoint = adminEndpoint;
            WAPAdminUserName = adminUserName;
            WAPAdminPassword = adminPassword;

            WAPAdminHeaderValue = WAPAuth.PerformADAuth(WAPAdminUserName, WAPAdminPassword);
            WAPAdmin.SetWAPEndpoint(WAPAdminEndpoint, WAPAdminUserName, WAPAdminHeaderValue);
            WAPAdmin.WAPServiceHttpClient();
        }

        /// <summary>
        /// Gets VMM context for a particular subscription
        /// </summary>
        /// <param name="subscriptionID"></param>
        public void GetVMMContextForUserRole(string userRoleName)
        {
            VMMAdmin = new VMMOperation();
            VMMAdmin.GetVMMContextForAdmin(WAPAdminEndpoint, userRoleName, WAPAdminHeaderValue);
        }

        /// <summary>
        /// Get Tenant user detail thru Admin endpoint
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public User GetWAPUser(string userID)
        {
            Uri requestUri = WAPAdmin.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, Constants.RelativePaths.User, userID));
            var users = WAPAdmin.GetWAPAsync<User>(requestUri);
            return users.Result;
        }

        /// <summary>
        /// creates a wap user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User> CreateUser(string username, string email)
        {
            Uri requestUri = WAPAdmin.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, Constants.RelativePaths.Users));
            var userInfo = new User()
            {
                Name = username,
                Email = email,
                State = UserState.Active,
            };
            return await WAPAdmin.SendAsync<User, User>(requestUri, new System.Net.Http.HttpMethod(Constants.HttpMethods.Post), userInfo); 
        }

        /// <summary>
        /// updates wap user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User> UpdateUser(string username, string email)
        {
            Uri requestUri = WAPAdmin.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, Constants.RelativePaths.User, username));
            var userInfo = new User()
            {
                Name = username,
                Email = email,
                State = UserState.Active,
            };
            return await WAPAdmin.SendAsync<User, User>(requestUri, new System.Net.Http.HttpMethod(Constants.HttpMethods.Put), userInfo); 
        }

        /// <summary>
        /// Gets all subscriptions for a user thru Admin endpoint
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public QueryResult<Subscription> GetSubscriptionsForTenant(string userID)
        {
            Uri requestUri = WAPAdmin.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, Constants.RelativePaths.Subscription, userID));
            var subscriptions = WAPAdmin.GetWAPAsync<QueryResult<Subscription>>(requestUri);
            return subscriptions.Result;
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
                    WAPAdmin = null;
                    VMMAdmin.Dispose();
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
