using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.WindowsAzure.Server.AdminManagement;
using Microsoft.WindowsAzure.Server.Management;
using Microsoft.WindowsAzure.Management.Models;
using WAPWrapper.SPFGallery;
using WAPWrapper.SPFVMM;
using WAPWrapper.SPFVMM.MicrosoftCompute;

namespace WAPWrapper
{
    /// <summary>
    /// A disposable class to perform Virtual Machine and other VMM operations (Cloud, User Role) for a User role (Administrator or Tenant or Self-Service User)
    /// </summary>
    public class VMMOperation : IDisposable
    {
        private string TenantUserName { get; set; }
        private string TenantUserRoleName { get; set; }
        private string TenantSubscriptionID { get; set; }
        private Guid TenantUserRoleID { get; set; }
        private string TenantHeaderValue { get; set; }
        private AuthType TenantAuthType { get; set; } 
        
        private static string WAPTenantEndpoint { get; set; }
        private static string WAPAdminEndpoint { get; set; }
        private static string VMMAdminUserRoleName { get; set; }
        private static string WAPAdminHeaderValue { get; set; }

        private VMMContext SPF { get; set; }
        private VMM VMMInstance { get; set; }
        private GalleryDataProvider GalleryInstance { get; set; }

        /// <summary>
        /// Instantiates VMOperation
        /// </summary>
        public VMMOperation()
        {
            SPF = VMMContext.Instance;
        }

        /// <summary>
        /// Gets VMM context for a particular subscription
        /// </summary>
        /// <param name="subscriptionID"></param>
        public void GetVMMContextForAdmin(string wapAdminEndpoint, string userRoleName, string adminHeaderValue)
        {
            WAPAdminEndpoint = wapAdminEndpoint;
            VMMAdminUserRoleName = userRoleName;
            WAPAdminHeaderValue = adminHeaderValue;

            SPF.SetVMMTenantContext(WAPAdminEndpoint, WAPAdminHeaderValue);

            VMMInstance = SPF.GetVMMContextForAdmin(VMMAdminUserRoleName);
        }

        /// <summary>
        /// Gets VMM context for a particular subscription
        /// </summary>
        /// <param name="subscriptionID"></param>
        public void GetVMMContextForTenant(string wapTenantEndpoint, string tenantUsername, string subscriptionID, string tenantHeaderValue)
        {
            WAPTenantEndpoint = wapTenantEndpoint;
            TenantUserName = tenantUsername;
            TenantSubscriptionID = subscriptionID;
            TenantHeaderValue = tenantHeaderValue;
            
            SPF.SetVMMTenantContext(WAPTenantEndpoint, TenantHeaderValue);

            TenantUserRoleName = string.Format("{0}_{1}", TenantUserName, TenantSubscriptionID);
            TenantUserRoleID = new Guid(TenantSubscriptionID);

            VMMInstance = SPF.GetVMMContextForTenant(TenantUserRoleName, TenantUserRoleID);

            GalleryInstance = SPF.GetGalleryContextForTenant(TenantUserRoleName, TenantUserRoleID);
        }

        /// <summary>
        /// Get all Clouds assigned to a User role
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VMRoleSizeProfile> GetVMRoleSizeProfiles()
        {
            return VMMInstance.VMRoleSizeProfiles;
        }

        /// <summary>
        /// Get all Clouds assigned to a User role
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VMRoleVMDisk> GetVMRoleVMDisk()
        {
            return VMMInstance.VMRoleVMDisk;
        }

        public IEnumerable<VMRoleVMNic> GetVMRoleVMNic()
        {
            return VMMInstance.VMRoleVMNic;
        }

        /// <summary>
        /// Get all Clouds assigned to a User role
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cloud> GetClouds()
        {
            return VMMInstance.Clouds;
        }

        /// <summary>
        /// Gets all VM Templates assigned to a User role
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VMTemplate> GetVMTemplate()
        {
            return VMMInstance.VMTemplates;
        }

        /// <summary>
        /// Create a VM under a Cloud using a VM template for a User role
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="cloudName"></param>
        /// <param name="templateName"></param>
        public void CreateVm(string vmName, string cloudName, string templateName)
        {
            var cloud = VMMInstance.Clouds.Where(t => t.Name == cloudName).First();
            var template = VMMInstance.VMTemplates.Where(q => q.Name == templateName && q.StampId == cloud.StampId).First();
            var vm = new VirtualMachine
                {
                    Name = vmName,
                    CloudId = cloud.ID,
                    StampId = cloud.StampId,
                    VMTemplateId = template.ID,
                };
            VMMInstance.AddToVirtualMachines(vm);
            VMMInstance.SaveChanges();
        }

        /// <summary>
        /// Stops a given VM for a User
        /// </summary>
        /// <param name="g"></param>
        public void StopVm(Guid g)
        {
            var vm = VMMInstance.VirtualMachines.Where(q => q.ID == g).First();
            vm.Operation = "Stop";
            VMMInstance.UpdateObject(vm);
            VMMInstance.SaveChanges();
        }

        /// <summary>
        /// Starts a given VM for a User
        /// </summary>
        /// <param name="g"></param>
        public void StartVm(Guid g)
        {
            var vm = VMMInstance.VirtualMachines.Where(q => q.ID == g).First();
            vm.Operation = "Start";
            VMMInstance.UpdateObject(vm);
            VMMInstance.SaveChanges();
        }

        /// <summary>
        /// Deletes a given VM for a User
        /// </summary>
        /// <param name="g"></param>
        public void DeleteVm(Guid g)
        {
            var vm = VMMInstance.VirtualMachines.Where(q => q.ID == g).First();
            VMMInstance.DeleteObject(vm);
            VMMInstance.SaveChanges();
        }

        /// <summary>
        /// Gets all VMs for a User
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VirtualMachine> GetVMs()
        {
            return VMMInstance.VirtualMachines;
        }

        /// <summary>
        /// Gets a VM By Name
        /// </summary>
        /// <param name="vmname"></param>
        /// <returns></returns>
        public VirtualMachine GetVMByName(string vmname)
        {
            return VMMInstance.VirtualMachines.Where(v => v.Name == vmname).FirstOrDefault();
        }

        /// <summary>
        /// Gets all VHDs by VM name
        /// </summary>
        /// <param name="vmname"></param>
        /// <returns></returns>
        public IEnumerable<VirtualHardDisk> GetVHDByVM(string vmname)
        {
            return VMMInstance.VirtualMachines.Expand("VirtualHardDisks").Where(v => v.Name == vmname).FirstOrDefault().VirtualHardDisks;
        }

        /// <summary>
        /// Gets all VHD by VM ID
        /// </summary>
        /// <param name="vmid"></param>
        /// <returns></returns>
        public IEnumerable<VirtualHardDisk> GetVHDByVMId(Guid? vmid)
        {
            return VMMInstance.VirtualMachines.Expand("VirtualHardDisks").Where(v => v.VMId == vmid).FirstOrDefault().VirtualHardDisks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<VirtualHardDisk> GetVHDByVMMId(Guid id)
        {
            return VMMInstance.VirtualMachines.Expand("VirtualHardDisks").Where(v => v.ID == id).FirstOrDefault().VirtualHardDisks;
        }

        /// <summary>
        /// Gets User role detail for a User
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserRole> GetUserRoles()
        {
            return VMMInstance.UserRoles;
        }

        /// <summary>
        /// Gets all Tenant Admins registered in VMM
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserRole> GetTenantAdmin()
        {
            return VMMInstance.UserRoles.Where(v => v.Profile == "TenantAdmin");
        }

        /// <summary>
        /// Gets all Self-Service Users registered in VMM
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserRole> GetSelfServiceUser()
        {
            return VMMInstance.UserRoles.Where(v => v.Profile == "SelfServiceUser");
        }

        /// <summary>
        /// Gets all VMNetworks for a User
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VMNetwork> GetVMNetwork()
        {
            return VMMInstance.VMNetworks;
        }

        /// <summary>
        /// Gets all LogicalNetworks for a User
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LogicalNetwork> GetLogicalNetwork()
        {
            return VMMInstance.LogicalNetworks;
        }

        /// <summary>
        /// Gets a Clod detail by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Cloud GetCloudByVMMId(Guid id)
        {
            return VMMInstance.VirtualMachines.Expand("Cloud").Where(v => v.ID == id).FirstOrDefault().Cloud;
        }

        /// <summary>
        /// Gets all network adapters by VM ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<VirtualNetworkAdapter> GetNetworkAdapterByVMMId(Guid id)
        {
            return VMMInstance.VirtualMachines.Expand("VirtualNetworkAdapters").Where(v => v.ID == id).FirstOrDefault().VirtualNetworkAdapters;
        }

        private bool disposed = false;

        /// <summary>
        /// Dispose the object for system/users call
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    SPF = null;
                    VMMInstance = null;
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Dispose the object per users call
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
