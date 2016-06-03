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
    class WAPASPAuth
    {
        public static string GetASPToken(string authSiteEndPoint, string userName, string password)
        {

            var identityProviderEndpoint = new EndpointAddress(new Uri(authSiteEndPoint + "/wstrust/issue/usernamemixed"));

            var identityProviderBinding = new WS2007HttpBinding(SecurityMode.TransportWithMessageCredential);
            identityProviderBinding.Security.Message.EstablishSecurityContext = false;
            identityProviderBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            identityProviderBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

            var trustChannelFactory = new WSTrustChannelFactory(identityProviderBinding, identityProviderEndpoint)
            {
                TrustVersion = TrustVersion.WSTrust13,
            };
            //This line is only if we're using self-signed certs in the installation 
            trustChannelFactory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication() { CertificateValidationMode = X509CertificateValidationMode.None };

            trustChannelFactory.Credentials.SupportInteractive = false;
            trustChannelFactory.Credentials.UserName.UserName = userName;
            trustChannelFactory.Credentials.UserName.Password = password;

            var channel = trustChannelFactory.CreateChannel();
            var rst = new RequestSecurityToken(RequestTypes.Issue)
            {
                AppliesTo = new EndpointReference("http://azureservices/TenantSite"),
                TokenType = "urn:ietf:params:oauth:token-type:jwt",
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
