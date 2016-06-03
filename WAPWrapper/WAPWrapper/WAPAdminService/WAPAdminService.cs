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
    public class WAPAdminService : WAPBaseService
    {
        private static string AdminUsername { get; set; }
        private static string AdminPassword { get; set; }

        private static string ADAuthEndpoint { get; set; }
        private static string ASPAuthEndpoint {get; set;}
        private static string ADFSAuthEndpoint { get; set; }
        private static string AdminEndpoint { get; set; }
        public  string AdminHeaderValue { get; set; }

        /// <summary>
        /// Constructor for WAPAdminContext
        /// </summary>
        public WAPAdminService()
            : base()
        {
        }

        /// <summary>
        /// Sets WAP Admin Context for Endpoint and Admin user credentials
        /// </summary>
        /// <param name="adminEndpoint"></param>
        /// <param name="adminUserName"></param>
        /// <param name="adminHeaderValue"></param>
        public void SetWAPEndpoint(string adminEndpoint, string adminUserName, string adminHeaderValue)
        {
            AdminUsername = adminUserName;
            AdminEndpoint = adminEndpoint;
            AdminHeaderValue = adminHeaderValue;
        }

        /// <summary>
        /// Method to set the httpClient for WAP Admin operations
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
            string WAPURL = string.Format("{0}/", AdminEndpoint);
            httpClient.BaseAddress = new UriBuilder(WAPURL).Uri;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AdminHeaderValue);
        }

    }
}