using ContosoPortal.Models;
using GistackWAPService.models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Security;

namespace GistackWAPService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    public class WapService : IWapService
    {
        private string authSiteEndPoint = "https://cloudd-zn.zncloud.com:30071";
        private string userName = "247638969@qq.com";
        private string password = "Esri@123";
        private WAPTenantContext wapContext { get; set; }

     

        public string getToken()
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(delegate { return true; });
            return GetAspAuthToken(this.authSiteEndPoint,this.userName,this.password);
        }

        public IEnumerable<Plan> GetPlans()
        {
            if(this.CheckAuthorization())
                return wapContext.GetWAPPlansForTenant();
            else
                return null;
 
        }

        public IEnumerable<Subscription> GetWAPSubscriptions()
        {
            if (this.CheckAuthorization())
                return wapContext.GetWAPSubscriptionForTenant();
            else
                return null;

        }

        public IEnumerable<Quota> GetQuotasForSubscription(string subscriptionid)
        {
            if (this.CheckAuthorization())
                return wapContext.GetWAPQuotaForSubscription(subscriptionid);
            else
                return null;

        }

        public IEnumerable<VMTemplate> GetWAPVMTemplates(string subscriptionid) 
        {
            if (this.CheckAuthorization())
            {
                wapContext.GetVMMContextForSubscription(subscriptionid);
                return wapContext.GetVMTemplate();
            }
            else
                return null;
        }

        public IEnumerable<VirtualMachine> GetVirtualMachines(string subscriptionid)
        {
            if (this.CheckAuthorization())
            {
                wapContext.GetVMMContextForSubscription(subscriptionid);
                return wapContext.GetVirtualMachine();
            }
            else
                return null;
        }

        public VirtualMachine GetVirtualMachine(string subscriptionid, string vmname) 
        {
            IEnumerable<VirtualMachine> vms = this.GetVirtualMachines(subscriptionid);
            if (vms != null && vms.Count() > 0)
            {
                IEnumerable<VirtualMachine> result = vms.Where(v => v.name.Contains(vmname));
                if (result != null && result.Count() > 0)
                   return result.ToList().First();
            }
            return null;
            
        }

        public IEnumerable<Cloud> GetWAPClouds(string subscriptionid)
        {
            if (this.CheckAuthorization())
            {
                wapContext.GetVMMContextForSubscription(subscriptionid);
                return wapContext.GetCloud();
            }
            else
                return null;
        }

        public IEnumerable<VMNetwork> GetWAPNetworks(string subscriptionid)
        {
            if (this.CheckAuthorization())
            {
                wapContext.GetVMMContextForSubscription(subscriptionid);
                return wapContext.GetVMNetworks();
            }
            else
                return null;
        }

        public Result CreateVirtualMachine(VirtualMachine vm, string subscriptionid) 
        {
            if (this.CheckAuthorization())
            {
               wapContext.GetVMMContextForSubscription(subscriptionid);
               IEnumerable<Cloud> clouds = wapContext.GetCloud();
               if (clouds != null)
               {
                   List<Cloud>listClouds = clouds.ToList();
                   if (listClouds.Count > 0 && listClouds[0] != null)
                   {
                       try
                       {
                           wapContext.CreateVM(vm.name, listClouds[0].name, vm.vmTemplate);
                           return new Result("true",null);
                       }
                       catch (Exception ex)
                       {
                           return new Result("false", ex.Message);
                       }
                   }
               }
            }
            return  new Result("false", "login deny");
        }

        public Result DeleteVirtualMachine(string guid, string subscriptionid)
        {
            if (this.CheckAuthorization())
            {
                wapContext.GetVMMContextForSubscription(subscriptionid);
                try
                {
                    Guid g = new Guid(guid);
                    wapContext.DeleteVM(g);
                    return new Result("true", null);
                }
                catch (Exception ex)
                {
                    return new Result("false", ex.Message);
                }
            }
            else
                return new Result("false", "login deny");
           
        }

        public Result StopVirtualMachine(string guid, string subscriptionid)
        {
            if (this.CheckAuthorization())
            {
                wapContext.GetVMMContextForSubscription(subscriptionid);
                try
                {
                    Guid g = new Guid(guid);
                    wapContext.StopVM(g);
                    return new Result("true", null);
                }
                catch (Exception ex)
                {
                    return new Result("false", ex.Message);
                }
            }
            else
                return new Result("false", "login deny");
        }

        public  Result StartVirtualMachine(string guid, string subscriptionid)
        {
            if (this.CheckAuthorization())
            {
                wapContext.GetVMMContextForSubscription(subscriptionid);
                try
                {
                    Guid g = new Guid(guid);
                    wapContext.StartVM(g);
                    return new Result("true", null);
                }
                catch (Exception ex)
                {
                    return new Result("false", ex.Message);
                }
            }
            else
                return new Result("false", "login deny");
        }


        public Login login(Login model)
        {
            string msg = string.Empty;
            try
            {
                WAPTenantContext wapContext = new WAPTenantContext();
                wapContext.SetWAPAuthContext(ConfigurationManager.AppSettings["WAPADAuthServer"],
                                             ConfigurationManager.AppSettings["WAPASPAuthServer"]);
                model.TenantAuthType = WAPWrapper.AuthType.ASPAuth;
                model = wapContext.CheckWAPUser(model);
                if (model.IsValidUser)
                {
                    LoginList.Instance.InsertLoginRecord(model);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
            return model;
        }

        private Login CheckUser(Login model)
        {
            string msg = string.Empty;
            try
            {
                wapContext = new WAPTenantContext();
                wapContext.SetWAPAuthContext(ConfigurationManager.AppSettings["WAPADAuthServer"],
                                             ConfigurationManager.AppSettings["WAPASPAuthServer"]);
                model.TenantAuthType = WAPWrapper.AuthType.ASPAuth;
                model = wapContext.CheckWAPUser(model);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                throw;
            }
            return model;

        }

        private bool CheckAuthorization()
        {
            var ctx = WebOperationContext.Current;
            var auth = ctx.IncomingRequest.Headers[HttpRequestHeader.Authorization];
            if (!string.IsNullOrEmpty(auth))
            {
                var username = auth.Substring(0, auth.IndexOf("/"));
                var password = auth.Remove(0, auth.IndexOf("/")+1);

                wapContext = new WAPTenantContext();
                wapContext.SetWAPAuthContext(ConfigurationManager.AppSettings["WAPADAuthServer"],
                                             ConfigurationManager.AppSettings["WAPASPAuthServer"]);
                Login model = new Login();
                model.UserName = username;
                model.Password = password;
                model.TenantAuthType = WAPWrapper.AuthType.ASPAuth;
                model = wapContext.CheckWAPUser(model);
                if (model.IsValidUser)
                    return true;
            }
            ctx.OutgoingResponse.StatusCode = HttpStatusCode.MethodNotAllowed;
            return false;
        }

        private string GetAspAuthToken(string authSiteEndPoint, string userName, string password)
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
