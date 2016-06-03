using GistackWAPService.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace GistackWAPService
{
     [ServiceContract]
    public interface IWapService
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "getToken", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string getToken();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "plans", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Plan> GetPlans();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "login", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Login login(Login model);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "subscriptions", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
         IEnumerable<Subscription> GetWAPSubscriptions();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "subscriptions/{subscriptionid}/quotas", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Quota> GetQuotasForSubscription(string subscriptionid);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "subscriptions/{subscriptionid}/vmtemplates", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<VMTemplate> GetWAPVMTemplates(string subscriptionid);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "subscriptions/{subscriptionid}/virtualmachines", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<VirtualMachine> GetVirtualMachines(string subscriptionid);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "subscriptions/{subscriptionid}/virtualmachines/{vmname}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        VirtualMachine GetVirtualMachine(string subscriptionid,string vmname);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "subscriptions/{subscriptionid}/clouds", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Cloud> GetWAPClouds(string subscriptionid);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "subscriptions/{subscriptionid}/vmnetworks", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<VMNetwork> GetWAPNetworks(string subscriptionid);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "subscriptions/{subscriptionid}/virtualmachine", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Result CreateVirtualMachine(VirtualMachine vm, string subscriptionid);

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "subscriptions/{subscriptionid}/virtualmachines/{guid}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Result DeleteVirtualMachine(string guid, string subscriptionid);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "subscriptions/{subscriptionid}/virtualmachines/{guid}/stop", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Result StopVirtualMachine(string guid, string subscriptionid);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "subscriptions/{subscriptionid}/virtualmachines/{guid}/start", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Result StartVirtualMachine(string guid, string subscriptionid);

    }
}