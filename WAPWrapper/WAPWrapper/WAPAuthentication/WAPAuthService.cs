using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace WAPWrapper
{
    /// <summary>
    /// A class to instantiate WAP Admin context to perform WAP Admin functions
    /// </summary>
    public class WAPAuthService
    {
        private static WAPAuthService wapAuthService = null;
        private static string ADAuthEndpoint { get; set; }
        private static string ASPAuthEndpoint {get; set;}
        private static string ADFSAuthEndpoint { get; set; }

        /// <summary>
        /// Constructor for WAPAdminContext
        /// </summary>
        private WAPAuthService()
        {
            //ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(delegate { return true; });
        }

        /// <summary>
        /// Sets WAP Auth Context for Endpoint
        /// </summary>
        /// <param name="adAuthEndpoint"></param>
        /// <param name="aspAuthEndpoint"></param>
        public void SetWAPAuthEndpoint(string adAuthEndpoint, string aspAuthEndpoint)
        {
            ADAuthEndpoint = adAuthEndpoint;
            ASPAuthEndpoint = aspAuthEndpoint;
        }

        /// <summary>
        /// Instantiates a single-instance of WAP Admin Context
        /// </summary>
        public static WAPAuthService Instance
        {
            get
            {
                if (wapAuthService == null)
                {
                    wapAuthService = new WAPAuthService();
                }

                return wapAuthService;
            }
        }

        /// <summary>
        /// Gets the security token for AD Authentication
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string PerformADAuth(string userName, string password)
        {
            string[] userpass = userName.Split('\\');
            string Domain = userpass[0];
            string User = userpass[1];

            string windowsAuthSiteEndPoint = string.Format("{0}", ADAuthEndpoint);
            string securityToken = WAPADAuth.GetAuthToken(windowsAuthSiteEndPoint, Domain, User, password, true);
            return securityToken;
        }

        /// <summary>
        /// Gets the security token for ASP Authentication
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string PerformASPAuth(string userName, string password)
        {
            string aspAuthSiteEndPoint = string.Format("{0}", ASPAuthEndpoint);
            string securityToken = WAPASPAuth.GetASPToken(aspAuthSiteEndPoint, userName, password);
            return securityToken;
        }

        /// <summary>
        /// Gets the security token for ADFS Authentication
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string PerformADFSAuth(string userName, string password)
        {
            string aspAuthSiteEndPoint = string.Format("{0}", ASPAuthEndpoint);
            string securityToken = WAPADFSAuth.GetADFSToken(aspAuthSiteEndPoint, userName, password);
            return securityToken;
        }

        /// <summary>
        /// Gets the security token for ADFSASP Authentication
        /// </summary>
        /// <param name="adfsEndpoint"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string PerformADFSASPAuth(string adfsEndpoint, string userName, string password)
        {
            string aspAuthSiteEndPoint = string.Format("{0}", ASPAuthEndpoint);
            string securityToken = WAPADFSASPAuth.GetADFSASPToken(adfsEndpoint, aspAuthSiteEndPoint, userName, password);
            return securityToken;
        }


    }
}