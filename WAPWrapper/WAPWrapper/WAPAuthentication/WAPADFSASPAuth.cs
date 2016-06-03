using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Security;
using Microsoft.WindowsAzure.Server.AdminManagement;
using Microsoft.WindowsAzure.Server.Management;
using System.Xml;

namespace WAPWrapper
{
    class WAPADFSASPAuth
    {
        public static string GetADFSASPToken(string ADFSEndpoint, string authSiteEndPoint, string userName, string password)
        {

            var identityProviderEndpoint = new EndpointAddress(new Uri(authSiteEndPoint + "/wstrust/issue/usernamemixed"));
            var federationEndpoint = new EndpointAddress(new Uri(ADFSEndpoint + "/adfs/services/trust/13/issuedtokenmixedasymmetricbasic256sha256"));//, identity);

            var identityProviderBinding = new WS2007HttpBinding(SecurityMode.TransportWithMessageCredential);
            identityProviderBinding.Security.Message.EstablishSecurityContext = false;
            identityProviderBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            identityProviderBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

            var xml = new XmlDocument();
            xml.LoadXml(@"<wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy""><wsa:EndpointReference xmlns:wsa=""http://www.w3.org/2005/08/addressing""><wsa:Address>http://kc-adfs.katalcloud.com/adfs/services/trust</wsa:Address></wsa:EndpointReference></wsp:AppliesTo>");
            var federationBinding = new WS2007FederationHttpBinding(WSFederationHttpSecurityMode.TransportWithMessageCredential);
            federationBinding.Security.Message.EstablishSecurityContext = false;
            federationBinding.Security.Message.IssuedKeyType = SecurityKeyType.AsymmetricKey;
            federationBinding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Basic256Sha256;
            federationBinding.Security.Message.NegotiateServiceCredential = false;
            federationBinding.Security.Message.TokenRequestParameters.Add(xml.DocumentElement);
            federationBinding.Security.Message.IssuerAddress = identityProviderEndpoint;
            federationBinding.Security.Message.IssuerBinding = identityProviderBinding;
            federationBinding.Security.Message.IssuedTokenType = "urn:oasis:names:tc:SAML:2.0:assertion";

            var trustChannelFactory = new WSTrustChannelFactory(federationBinding, federationEndpoint)
            {
                TrustVersion = TrustVersion.WSTrust13,
            };

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

            var token = channel.Issue(rst, out rstr);
            var tokenString = (token as GenericXmlSecurityToken).TokenXml.InnerText;
            var jwtString = Encoding.UTF8.GetString(Convert.FromBase64String(tokenString));
            //Console.WriteLine(jwtString);

            return jwtString;

        }
    }
}
