using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Server.AdminManagement;
using Microsoft.WindowsAzure.Server.Management;
using Microsoft.WindowsAzure.Management.Models;
using System.Net;
using System.Net.Security;
using WAPWrapper;
using GistackWAPService.models;
using System.Configuration;

namespace ContosoPortal.Models
{
    /// <summary>
    /// A disposable object to instantiate VMM Context for a User to perform VMM operations
    /// </summary>
    public class WAPTenantContext : IDisposable
    {
        private WAPTenantOperation WAPOperation { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private string UserRoleName { get; set; }

        private string TenantUserName { get; set; }
        private string TenantPassword { get; set; }
        private AuthType TenantAuthType { get; set; }
        private string SubscriptionID { get; set; }
        private string WAPADAuthServer { get; set; }
        private string WAPASPAuthServer { get; set; }
        private string WAPAdminServer { get; set; }
        private string WAPTenantServer { get; set; }
        private string WAPAdminUserName { get; set; }
        private string WAPAdminPassword { get; set; }

        /// <summary>
        /// Contructor for WAPTenantContext
        /// </summary>
        public WAPTenantContext()
        {
            if (WAPOperation == null)
            {
                WAPOperation = new WAPTenantOperation();
            }
        }

        /// <summary>
        /// Sets the WAP Auth context endpoints
        /// </summary>
        /// <param name="adAuthEndpoint"></param>
        /// <param name="aspAuthEndpoint"></param>
        public void SetWAPAuthContext(string adAuthEndpoint, string aspAuthEndpoint)
        {
            this.WAPADAuthServer = adAuthEndpoint;
            this.WAPASPAuthServer = aspAuthEndpoint;

            WAPOperation.SetWAPAuthContext(WAPADAuthServer, WAPASPAuthServer);
        }

        /// <summary>
        /// Sets the WAP Tenant context for authentication, tenant credential
        /// </summary>
        /// <param name="tenantEndpoint"></param>
        /// <param name="authenticationType"></param>
        /// <param name="tenantUserName"></param>
        /// <param name="tenantPassword"></param>
        public void SetWAPTenantContext(string tenantEndpoint, AuthType authenticationType, string tenantUserName, string tenantPassword)
        {
            this.TenantUserName = tenantUserName;
            this.TenantPassword = tenantPassword;
            this.TenantAuthType = authenticationType;
            this.WAPTenantServer = tenantEndpoint;

            WAPOperation.SetWAPTenantContext(WAPTenantServer, TenantAuthType, TenantUserName, TenantPassword);
        }

        /// <summary>
        /// Check whether user is a valid WAP user or not
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Login CheckWAPUser(Login login)
        {
            try 
            {
                String wapserver = ConfigurationManager.AppSettings["WAPServer"];
                SetWAPTenantContext(wapserver, login.TenantAuthType, login.UserName, login.Password);
                var user = WAPOperation.GetTenantUser();
                if (user != null)
                {
                    login.IsValidUser = true;
                }
                return login;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets VMM Context for a subscription
        /// </summary>
        /// <param name="subscriptionID"></param>
        public void GetVMMContextForSubscription(string subscriptionID)
        {
            try
            {
                this.SubscriptionID = subscriptionID;
                WAPOperation.GetVMMContextForSubscription(subscriptionID);
            }
            catch (Exception ex)
            {
                // Log the exception.
                throw;
            }
        }

        /// <summary>
        /// Gets WAP Plans for a tenant
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IEnumerable<GistackWAPService.models.Plan> GetWAPPlansForTenant()
        {
            IEnumerable<GistackWAPService.models.Plan> query = null;
            try
            {
                var plans = WAPOperation.GetAvailablePlans();
                query = plans.Select(s =>
                    new GistackWAPService.models.Plan
                    {
                        PlanId = s.Id,
                        PlanDisplayName = s.DisplayName,
                        PlanPrice = s.Price
                    });
            }
            catch (Exception)
            {
                throw;
            }
            return query;
        }

        /// <summary>
        /// Gets WAP subscription for tenant
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GistackWAPService.models.Subscription> GetWAPSubscriptionForTenant()
        {
            IEnumerable<GistackWAPService.models.Subscription> query = null;
            try
            {
                query = WAPOperation.GetSubscriptions().Select(s =>
                new GistackWAPService.models.Subscription
                        {
                        subscriptionID = s.SubscriptionID,
                        name = s.SubscriptionName,
                        offerCategory = s.OfferCategory,
                        offerFriendlyName = s.OfferFriendlyName,
                        planId = s.PlanId,
                        registeredServices = s.RegisteredServices
                        });
            }
            catch (Exception)
            {
                throw;
            }
            return query;
        }

        /// <summary>
        /// Gets WAP subscription for tenant
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GistackWAPService.models.Quota> GetWAPQuotaForSubscription(string subscriptionID)
        {
            IList<GistackWAPService.models.Quota> query = new List<GistackWAPService.models.Quota>();
            try
            {
                var quotaList = WAPOperation.GetTenantQuotaForSubscription(subscriptionID).Select(x => x.Usages.Select(y =>
                new GistackWAPService.models.Quota
                {
                    currentValue = y.CurrentValue,
                    displayName = y.DisplayName,
                    groupDisplayName = y.GroupDisplayName,
                    groupId = y.GroupId,
                    limit = y.Limit,
                    limitString = y.Limit == -1 ? "Unlimited" : y.Limit.ToString(),
                    unitDisplayName = y.UnitDisplayName
                }));
                foreach (var item in quotaList)
                {
                    foreach (var quota in item)
                    {
                         query.Add(quota);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return query;
        }

        /// <summary>
        /// Creates a Subscription for the tenant
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="cloudName"></param>
        /// <param name="templateName"></param>
        public async Task CreateSubscription(string planId, string planName)
        {
            try
            {
                var subs = await WAPOperation.CreateSubscription(planId, planName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a subscription for a tenant
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public async Task UpdateSubscription(GistackWAPService.models.Subscription subscription)
        {
            try
            {
                var subs = await WAPOperation.UpdateSubscriptionNameAsync(subscription.subscriptionID, subscription.name);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all VMRoleSizeProfiles registered in VMM
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GistackWAPService.models.VMRoleSizeProfile> GetVMRoleSizeProfile()
        {
            IEnumerable<GistackWAPService.models.VMRoleSizeProfile> query = null;
            try
            {
                query = WAPOperation.VMMTenant.GetVMRoleSizeProfiles().Select(v =>
                        new GistackWAPService.models.VMRoleSizeProfile
                        {
                            Name = v.Name,
                            CPU = v.CpuCount
                        }
                        );
            }
            catch (Exception)
            {
                throw;
            }
            return query;
        }

        /// <summary>
        /// Gets VM detail for tenant
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GistackWAPService.models.VirtualMachine> GetVirtualMachine()
        {
            var query = new List<GistackWAPService.models.VirtualMachine>();
            try
            {
                var vms = WAPOperation.VMMTenant.GetVMs();
                foreach (var vm in vms)
                {
                    string cloudname = string.Empty;
                    var cloud = WAPOperation.VMMTenant.GetCloudByVMMId(vm.ID);
                    if (cloud != null)
                    {
                        cloudname = cloud.Name;
                    };
                    var vhds = WAPOperation.VMMTenant.GetVHDByVMMId(vm.ID);
                    var networkadapters = WAPOperation.VMMTenant.GetNetworkAdapterByVMMId(vm.ID);
                    query.Add(
                        new GistackWAPService.models.VirtualMachine
                        {
                            id = vm.ID,
                            name = vm.Name,
                            cloud = cloudname,
                            computerName = vm.ComputerName,
                            cpuCount = (int)vm.CPUCount,
                            memoryAssignedMB = (int)vm.MemoryAssignedMB,
                            dynamicMemoryMinimumMB = 0,
                            perfCPUUtilization = (int)vm.PerfCPUUtilization,
                            perfMemory = (int)vm.Memory,
                            virtualMachineStatus = vm.Status,
                            operatingSystem = vm.OperatingSystem,
                            ips = networkadapters.Select(v => v.IPv4Addresses).FirstOrDefault(),
                            virtualHardDisks = vhds.Select(v => v.Name),
                            virtualNetworkAdapters = networkadapters.Select(v => v.VMNetworkName)
                        });
                }
            }
            catch (Exception)
            {
                throw;
            }
            return query;
        }

        /// <summary>
        /// Gets Cloud detail for tenant
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GistackWAPService.models.Cloud> GetCloud()
        {
            IEnumerable<GistackWAPService.models.Cloud> query = null;
            try
            {
                query = WAPOperation.VMMTenant.GetClouds().Select(v =>
                        new GistackWAPService.models.Cloud
                        {
                            id = v.ID,
                            name = v.Name
                        }
                        );
            }
            catch (Exception)
            {
                throw;
            }
            return query;
        }

        /// <summary>
        /// Gets VMTemplate detail for tenant
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GistackWAPService.models.VMTemplate> GetVMTemplate()
        {
            IEnumerable<GistackWAPService.models.VMTemplate> query = null;
            try
            {
                query = WAPOperation.VMMTenant.GetVMTemplate().Select(v =>
                        new GistackWAPService.models.VMTemplate
                        {
                            id = v.ID,
                            name = v.Name
                        }
                        );
            }
            catch (Exception)
            {
                throw;
            }
            return query;
        }


        /// <summary>
        /// Gets VMNetwork detail for tenant
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GistackWAPService.models.VMNetwork> GetVMNetworks()
        {
            IEnumerable<GistackWAPService.models.VMNetwork> query = null;
            try
            {
                query = WAPOperation.VMMTenant.GetVMNetwork().Select(v =>
                        new GistackWAPService.models.VMNetwork
                        {
                            id = v.ID,
                            name = v.Name
                        }
                        );
            }
            catch (Exception)
            {
                throw;
            }
            return query;
        }

        /// <summary>
        /// Gets VMNetwork detail for tenant
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GistackWAPService.models.LogicalNetwork> GetLogicalNetwork()
        {
            IEnumerable<GistackWAPService.models.LogicalNetwork> query = null;
            try
            {
                query = WAPOperation.VMMTenant.GetLogicalNetwork().Select(v =>
                        new GistackWAPService.models.LogicalNetwork
                        {
                            id = v.ID,
                            name = v.Name,
                        }
                        );
            }
            catch (Exception)
            {
                throw;
            }
            return query;
        }


        /// <summary>
        /// Gets VMM side statistics for Cloud, VM
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GistackWAPService.models.VMMStatistics> GetVMMStatistics()
        {
            IEnumerable<GistackWAPService.models.VMMStatistics> query = null;
            try
            {
                var virtualmachines = GetVirtualMachine();
                query = from vmms in virtualmachines
                            group vmms by vmms.cloud into g
                            orderby g.Count() descending
                        select new GistackWAPService.models.VMMStatistics { CloudName = g.Key, VMCount = g.Count() };
            }
            catch (Exception)
            {
                throw;
            }
            return query;
        }
    
        /// <summary>
        /// Stops a VM per VM ID
        /// </summary>
        /// <param name="id"></param>
        public void StopVM(Guid id)
        {
            try
            {
                WAPOperation.VMMTenant.StopVm(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Starts a VM per VM ID
        /// </summary>
        /// <param name="id"></param>
        public void StartVM(Guid id)
        {
            try
            {
                WAPOperation.VMMTenant.StartVm(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a VM for the tenant
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="cloudName"></param>
        /// <param name="templateName"></param>
        public void CreateVM(string vmName, string cloudName, string templateName)
        {
            try
            {
                WAPOperation.VMMTenant.CreateVm(vmName, cloudName, templateName);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void DeleteVM(Guid guid)
        {
            try
            {
                WAPOperation.VMMTenant.DeleteVm(guid);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool disposed = false;

        /// <summary>
        /// method to dispose the object per system/user call
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    WAPOperation = null;
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// method to dispose object called by user
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// method to check the object is disposed or not
        /// </summary>
        /// <returns></returns>
        public bool IsDisposed()
        {
            return disposed;
        }
    }
}
