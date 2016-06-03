using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Services.Client;
using System.Net;
using System.Net.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using WAPWrapper.SPFGallery;
using WAPWrapper.SPFVMM;

namespace WAPWrapper
{
    /// <summary>
    /// Class to hols the VMM instance for Tenants, SelfServiceUsers and Administrators
    /// </summary>
    public class VMMContext
    {
        private static VMMContext vmmContext = null;
        private static string AdminEndpoint { get; set; }
        public string AdminHeaderValue { get; set; }
        private static string AdminUserroleName { get; set; }

        private string TenantUserName { get; set; }
        private string TenantSubscriptionID { get; set; }
        private string TenantUserRoleName { get; set; }
        public string TenantHeaderValue { get; set; }
        private string TenantEndpoint { get; set; }

        private string WAPServer { get; set; }

        private VMM VMMInstance { get; set; }
        private static object sync = new object();
        private static Dictionary<string, VMM> VmmContextCache { get; set; }
        private static Dictionary<string, GalleryDataProvider> GalleryContextCache { get; set; }

        /// <summary>
        /// Constructor for VMMContext instance
        /// </summary>
        private VMMContext()
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(delegate { return true; });

            AdminUserroleName = "Administrator";
        }

        /// <summary>
        /// A singleton instance to hold VMM instance for all logged-in users
        /// </summary>
        public static VMMContext Instance
        {
            get
            {
                if (vmmContext == null)
                {
                    vmmContext = new VMMContext();
                }

                return vmmContext;
            }
        }

        /// <summary>
        /// Method to set the VMMContext for Admin access
        /// </summary>
        /// <param name="adminEndpoint"></param>
        /// <param name="adminHeaderValue"></param>
        public void SetVMMAdminContext(string adminEndpoint, string adminHeaderValue)
        {
            AdminEndpoint = adminEndpoint;
            AdminHeaderValue = adminHeaderValue;
        }

        /// <summary>
        /// Method to set the VMMContext for Tenant access
        /// </summary>
        /// <param name="tenantEndpoint"></param>
        /// <param name="tenantHeaderValue"></param>
        public void SetVMMTenantContext(string tenantEndpoint, string tenantHeaderValue)
        {
            TenantEndpoint = tenantEndpoint;
            TenantHeaderValue = tenantHeaderValue;
        }

        /// <summary>
        /// Method to check a Tenant user is valid or not in VMM
        /// </summary>
        /// <param name="tenantUserRoleName"></param>
        /// <returns></returns>
        public bool IsValidUserRole(string tenantUserRoleName)
        {
            var userRole = GetUserRole(tenantUserRoleName);
            if (userRole != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to get VMM User role object
        /// </summary>
        /// <param name="userRoleName"></param>
        /// <returns></returns>
        private UserRole GetUserRole(string userRoleName)
        {
            UserRole loggedInUserRole = null;
            var userRoles = VmmContextInstance(AdminUserroleName, Guid.Empty).UserRoles.Where(q => q.Name == userRoleName).ToArray();
            if (userRoles.Count() == 1)
            {
                loggedInUserRole = userRoles.First();
            }
            return loggedInUserRole;
        }

        /// <summary>
        /// Method to get VMM Instance for a Tenant User
        /// </summary>
        /// <param name="tenantUserRoleName"></param>
        /// <returns></returns>
        public VMM GetVMMContextForAdmin(string userRoleName)
        {
            UserRole userRole = GetUserRole(userRoleName);
            if (userRole != null)
            {
                return VmmContextInstance(userRoleName, userRole.ID);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Method to get VMM Instance for a Tenant user with user role ID / Subscription
        /// </summary>
        /// <param name="tenantUserRoleName"></param>
        /// <param name="userRoleID"></param>
        /// <returns></returns>
        public VMM GetVMMContextForTenant(string tenantUserRoleName, Guid userRoleID)
        {
            return VmmContextInstance(tenantUserRoleName, userRoleID);
        }

        /// <summary>
        /// Method to get VMM Instance for a Tenant user with user role ID / Subscription
        /// </summary>
        /// <param name="tenantUserRoleName"></param>
        /// <param name="userRoleID"></param>
        /// <returns></returns>
        public GalleryDataProvider GetGalleryContextForTenant(string tenantUserRoleName, Guid userRoleID)
        {
            return GalleryContextInstance(tenantUserRoleName, userRoleID);
        }

        /// <summary>
        /// Method to set the Authorization token in request header for WAP Admin Authentication
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSendingRequestAdmin(object sender, SendingRequestEventArgs e)
        {
            // Add an Authorization header that contains an OAuth WRAP access token to the request.
            if (AdminHeaderValue != null)
            {
                string headerValue = "Bearer " + AdminHeaderValue;
                e.RequestHeaders.Add("Authorization", headerValue);
            }
        }

        /// <summary>
        /// Method to set the Authorization token in request header for WAP Tenant Authentication
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSendingRequestTenant(object sender, SendingRequestEventArgs e)
        {
            // Add an Authorization header that contains an OAuth WRAP access token to the request.
            if (TenantHeaderValue != null)
            {
                string headerValue = "Bearer " + TenantHeaderValue;
                e.RequestHeaders.Add("Authorization", headerValue);
                e.RequestHeaders.Add("x-ms-principal-id", string.Format("[{0}]", TenantUserName));
            }
            else
            {
                e.RequestHeaders.Add("x-ms-principal-id", "[test]");
            }
        }

        /// <summary>
        /// Method to set the Authorization token in request header for WAP Tenant Authentication
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSendingRequestGallery(object sender, SendingRequest2EventArgs e)
        {
            // Add an Authorization header that contains an OAuth WRAP access token to the request.
            if (TenantHeaderValue != null)
            {
                string headerValue = "Bearer " + TenantHeaderValue;
                e.RequestMessage.SetHeader("Authorization", headerValue);
                e.RequestMessage.SetHeader("x-ms-principal-id", string.Format("[{0}]", TenantUserName));
            }
            else
            {
                e.RequestMessage.SetHeader("x-ms-principal-id", "[test]");
            }
        }

        /// <summary>
        /// Get VMM Instance for Administrator
        /// </summary>
        /// <returns></returns>
        private VMM VmmContextInstance()
        {
            return VmmContextInstance(AdminUserroleName, Guid.Empty);
        }

        /// <summary>
        /// Gets VMM Instance for a User role (Tenant, Self-Service User or Administrator)
        /// </summary>
        /// <param name="userRoleName"></param>
        /// <param name="userRoleID"></param>
        /// <returns></returns>
        private VMM VmmContextInstance(string userRoleName, Guid userRoleID)
        {
            if (VmmContextCache == null || !VmmContextCache.ContainsKey(userRoleName))
            {
                lock (sync)
                {
                    if (VmmContextCache == null)
                    {
                        VmmContextCache = new Dictionary<string, VMM>();
                    }

                    if (!VmmContextCache.ContainsKey(userRoleName))
                    {
                        VMM context = null;
                        if (userRoleName != "Administrator")
                        {
                            // Create subscription auth mode context
                            context = new VMM(new Uri(string.Format("{0}/{1}/services/systemcenter/VMM/Microsoft.Management.Odata.svc/", TenantEndpoint, userRoleID.ToString().Replace("{", "").Replace("}", ""))));
                            context.SendingRequest += new EventHandler<SendingRequestEventArgs>(OnSendingRequestTenant);
                        }
                        else
                        {
                            // Create admin auth mode context
                            context = new VMM(new Uri(string.Format("{0}/services/systemcenter/SC2012R2/VMM/Microsoft.Management.Odata.svc/", AdminEndpoint)));
                            context.SendingRequest += new EventHandler<SendingRequestEventArgs>(OnSendingRequestAdmin);
                        }

                        // use OverwriteChanges to see updates made by VMM
                        context.MergeOption = System.Data.Services.Client.MergeOption.OverwriteChanges;
                        VmmContextCache.Add(userRoleName, context);
                        return context;
                    }
                }
            }
            return VmmContextCache[userRoleName];
        }

        /// <summary>
        /// Get VMM Instance for Administrator
        /// </summary>
        /// <returns></returns>
        private GalleryDataProvider GalleryContextInstance()
        {
            return GalleryContextInstance(AdminUserroleName, Guid.Empty);
        }

        /// <summary>
        /// Gets VMM Instance for a User role (Tenant, Self-Service User or Administrator)
        /// </summary>
        /// <param name="userRoleName"></param>
        /// <param name="userRoleID"></param>
        /// <returns></returns>
        private GalleryDataProvider GalleryContextInstance(string userRoleName, Guid userRoleID)
        {
            if (GalleryContextCache == null || !GalleryContextCache.ContainsKey(userRoleName))
            {
                lock (sync)
                {
                    if (GalleryContextCache == null)
                    {
                        GalleryContextCache = new Dictionary<string, GalleryDataProvider>();
                    }

                    if (!GalleryContextCache.ContainsKey(userRoleName))
                    {
                        GalleryDataProvider context = null;
                        if (userRoleName != "Administrator")
                        {
                            // Create subscription auth mode context
                            string uriString = string.Format("{0}/{1}/gallery/", TenantEndpoint, userRoleID.ToString().Replace("{", "").Replace("}", ""));

                            context = new GalleryDataProvider(new Uri(uriString));
                            context.BuildingRequest += (sender, e) =>
                            {
                                UriBuilder builder = new UriBuilder(e.RequestUri);

                                if (builder.Query != "")
                                    builder.Query = builder.Query.TrimStart('?') + "&api-version=2013-03";
                                else
                                    builder.Query = "api-version=2013-03";

                                e.RequestUri = builder.Uri;
                            };

                            //context = new GalleryDataProvider(new Uri(string.Format("{0}/services/systemcenter/VMM/GalleryService.svc/", TenantEndpoint, userRoleID.ToString().Replace("{", "").Replace("}", ""))));
                            context.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(OnSendingRequestGallery);
                        }
                        else
                        {
                            // Create admin auth mode context
                            context = new GalleryDataProvider(new Uri(string.Format("{0}/gallery/SC2012R2/VMM/GalleryService.svc/", AdminEndpoint)));
                            context.SendingRequest += new EventHandler<SendingRequestEventArgs>(OnSendingRequestAdmin);
                        }

                        // use OverwriteChanges to see updates made by VMM
                        context.MergeOption = System.Data.Services.Client.MergeOption.OverwriteChanges;
                        GalleryContextCache.Add(userRoleName, context);
                        return context;
                    }
                }
            }
            return GalleryContextCache[userRoleName];
        }
    }
}
