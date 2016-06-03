using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;

namespace WAPWrapper
{
    public class WAPADAuth
    {

        public static string GetAuthToken(string windowsAuthSiteEndpoint, string domainName, string userName, string password, bool shouldImpersonate)
        {
            string token = null;


            Impersonation.Impersonate(domainName, userName, password, () => token = GetWindowsToken(windowsAuthSiteEndpoint));
            return token;
        }

        private static string GetWindowsToken(string windowsAuthSiteEndPoint)
        {
            var identityProviderEndpoint = new EndpointAddress(new Uri(windowsAuthSiteEndPoint + "/wstrust/issue/windowstransport"));
            var identityProviderBinding = new WS2007HttpBinding(SecurityMode.Transport);
            identityProviderBinding.Security.Message.EstablishSecurityContext = false;
            identityProviderBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            identityProviderBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            var trustChannelFactory = new WSTrustChannelFactory(identityProviderBinding, identityProviderEndpoint)
            {
                TrustVersion = TrustVersion.WSTrust13,
            };

            trustChannelFactory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication() { CertificateValidationMode = X509CertificateValidationMode.None };
            var channel = trustChannelFactory.CreateChannel();

            var rst = new RequestSecurityToken(RequestTypes.Issue)
            {
                AppliesTo = new EndpointReference("http://azureservices/AdminSite"),
                KeyType = KeyTypes.Bearer,
            };

            RequestSecurityTokenResponse rstr = null;
            SecurityToken token = null;

            token = channel.Issue(rst, out rstr);
            var tokenString = (token as GenericXmlSecurityToken).TokenXml.InnerText;
            var jwtString = Encoding.UTF8.GetString(Convert.FromBase64String(tokenString));

            return jwtString;
        }
    }
}
