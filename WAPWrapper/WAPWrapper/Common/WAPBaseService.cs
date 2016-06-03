using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WAPWrapper
{
    /// <summary>
    /// A class to instantiate WAP Tenant context to perform WAP Tenant functions
    /// </summary>
    public class WAPBaseService
    {
        protected string WAPToken { get; set; }
        protected Uri WAPBaseURL { get; set; }
        protected HttpClient httpClient { get; set; }
        protected string mediaType { get; set; }

        protected const string JSONMediaType = "application/json";
        protected const string XMLMediaType = "application/atom+xml";

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementClientBase"/> class.
        /// </summary>
        /// <param name="baseEndpoint">The base endpoint.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="handler">Message processing handler</param>
        public WAPBaseService()
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(delegate { return true; });
        }


        /// <summary>
        /// Creates the request URI.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="queryStringParameters">The query string parameters.</param>
        /// <returns>Request URI</returns>
        public Uri CreateRequestUri(string relativePath, params KeyValuePair<string, string>[] queryStringParameters)
        {
            string queryString = string.Empty;

            if (queryStringParameters != null && queryStringParameters.Length > 0)
            {
                NameValueCollection queryStringProperties = System.Web.HttpUtility.ParseQueryString(httpClient.BaseAddress.Query);
                foreach (KeyValuePair<string, string> queryStringParameter in queryStringParameters)
                {
                    queryStringProperties[queryStringParameter.Key] = queryStringParameter.Value;
                }

                queryString = queryStringProperties.ToString();
            }

            return this.CreateRequestUri(relativePath, queryString);
        }

        /// <summary>
        /// Creates the request URI.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="queryString">The query string.</param>
        /// <returns>Request URI</returns>
        protected Uri CreateRequestUri(string relativePath, string queryString)
        {
            var endpoint = new Uri(httpClient.BaseAddress, relativePath);
            var uriBuilder = new UriBuilder(endpoint) { Query = queryString };
            return uriBuilder.Uri;
        }

        public Task<T> GetWAPAsync<T>(Uri requestUri, string userId = null)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                message.Headers.Add(Constants.Headers.PrincipalId, HttpUtility.UrlEncode(userId));
            }

            using (HttpResponseMessage response = this.httpClient.SendAsync(message).Result)
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Error Reading HTTP data...." + response.StatusCode);
                }

                return response.Content.ReadAsAsync<T>();
            }
        }

        /// <summary>
        /// Sends the async.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="userId">The user id. Only required by the tenant API.</param>
        /// <returns>Async task.</returns>
        public Task SendAsync(Uri requestUri, HttpMethod httpMethod, string userId = null)
        {
            return this.SendAsync<object>(requestUri, httpMethod, userId);
        }

        /// <summary>
        /// Sends the async.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Asyc task.</returns>
        public Task<TOutput> SendAsync<TOutput>(Uri requestUri, HttpMethod httpMethod, string userId = null)
        {
            var message = new HttpRequestMessage(httpMethod, requestUri);
            return this.SendAsync<TOutput>(requestUri, httpMethod, message, true, userId);
        }

        /// <summary>
        /// Sends the async.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="body">The body.</param>
        /// <param name="userId">The user id. Only required by the tenant API.</param>
        /// <returns>Asyc task.</returns>
        public Task SendAsync<TInput>(Uri requestUri, HttpMethod httpMethod, TInput body, string userId = null)
        {
            return this.SendAsync<TInput, object>(requestUri, httpMethod, body, userId);
        }

        /// <summary>
        /// Sends the async.
        /// </summary>
        /// <typeparam name="TInput">Input type.</typeparam>
        /// <typeparam name="TOutput">Output type.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="body">The body.</param>
        /// <param name="userId">The user id. Only required by the tenant API.</param>
        /// <returns>Asyc task.</returns>
        public Task<TOutput> SendAsync<TInput, TOutput>(Uri requestUri, HttpMethod httpMethod, TInput body, string userId = null, bool sendJSON = true)
        {
            var message = new HttpRequestMessage(httpMethod, requestUri)
            {
                Content = new ObjectContent<TInput>(body, this.CreateMediaTypeFormatter(sendJSON))
            };

            return this.SendAsync<TOutput>(requestUri, httpMethod, message, true, userId);
        }

        private async Task<TOutput> SendAsync<TOutput>(Uri requestUri, HttpMethod httpMethod, HttpRequestMessage message, bool hasResult, string userId = null)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                message.Headers.Add(Constants.Headers.PrincipalId, userId);
            }

            using (HttpResponseMessage response = await this.httpClient.SendAsync(message))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Error Reading HTTP data...." + response.StatusCode);
                }

                if (!hasResult)
                {
                    return default(TOutput);
                }

                return await response.Content.ReadAsAsync<TOutput>();
            }
        }

        private MediaTypeFormatter CreateMediaTypeFormatter(bool sendJSON)
        {
            MediaTypeFormatter formatter;

            if (sendJSON)
            {
                formatter = new JsonMediaTypeFormatter();
            }
            else
            {
                formatter = new XmlMediaTypeFormatter();
            }

            return formatter;
        }

        public async Task<String> SendAsyncString(Uri requestUri, HttpMethod httpMethod, string body, string principalUserId = null, bool sendJSON = true)
        {
            HttpRequestMessage message = new HttpRequestMessage(httpMethod, requestUri);

            StringContent content = new StringContent(body);

            // Setup http headers
            if (!string.IsNullOrWhiteSpace(principalUserId))
            {
                message.Headers.Add(Constants.Headers.PrincipalId, principalUserId);
            }

            // Setup the request message
            content.Headers.Remove("Content-Type");
            if (sendJSON)
            {
                content.Headers.Add("Content-Type", "application/json");
            }
            else
            {
                content.Headers.Add("Content-Type", "application/atom+xml");
            }

            message.Content = content;

            // Call the service
            var response = await this.httpClient.SendAsync(message);

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        public async Task<string> SendAsyncString(Uri requestUri, HttpMethod httpMethod, string principalUserId=null)
        {
            HttpRequestMessage message = new HttpRequestMessage(httpMethod, requestUri);

            // Setup http headers
            if (!string.IsNullOrWhiteSpace(principalUserId))
            {
                message.Headers.Add(Constants.Headers.PrincipalId, principalUserId);
            }

            // Call the service
            var response = await this.httpClient.SendAsync(message);

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}