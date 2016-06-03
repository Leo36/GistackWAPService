//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace WAPWrapper
{
    public static class Constants
    {
        internal static class DataContractNamespaces
        {
            // We are using the same namespaces as Azure for compatibility.
            public const string Default = "http://schemas.microsoft.com/windowsazure";

            public const string Provisioning = "http://www.microsoft.com/Azure/ProvisioningAgent/1.0";

            public const string ServiceManagement = "http://schemas.microsoft.com/2009/05/WindowsAzure/ServiceManagement";
        }

        internal static class DataContractNames
        {
            public const string SettingBatch = "SettingBatch";
        }

        internal static class ParameterNames
        {
            public const string SubscriptionId = "SubscriptionId";
            public const string HeaderPrincipalId = "HeaderPrincipalId";
            public const string SubscriptionCertificate = "SubscriptionCertificate";
            public const string SubscriptionCertificateThumbprint = "SubscriptionCertificateThumbprint";
            public const string SubscriptionCertificatePublicKey = "SubscriptionCertificatePublicKey";
            public const string SubscriptionCertificateData = "SubscriptionCertificateData";
        }

        internal static class Headers
        {
            public const string PrincipalId = "x-ms-principal-id";
        }

        internal static class HttpMethods
        {
            public const string Patch = "PATCH";
            public const string Post = "POST";
            public const string Get = "GET";
            public const string Put = "PUT";
            public const string Delete = "DELETE";
        }

        /// <summary>
        /// Relative paths class
        /// </summary>
        internal static class RelativePaths
        {
            public const string ResourceProviders = "resourceproviders";
            public const string Subscriptions = "subscriptions";
            public const string Plans = "plans";
            public const string AddOns = "addons";
            public const string Users = "users";
            public const string Settings = "settings";
            public const string Certificates = Subscription + "/certificates";

            public const string ResourceProvider = ResourceProviders + "/{0}/{1}";
            public const string Subscription = Subscriptions + "/{0}";
            public const string SubscriptionQuota = Subscriptions + "/{0}" + "/quota";
            public const string SubscriptionAddOns = Subscription + "/addons";
            public const string SubscriptionAddOn = SubscriptionAddOns + "/{1}/{2}";
            public const string SubscriptionServices = Subscription + "/services";
            public const string SubscriptionService = SubscriptionServices + "/{1}";
            public const string SubscriptionUsageSummaries = Subscription + "/usagesummaries";
            public const string Plan = Plans + "/{0}";
            public const string PlanQuota = Plan + "/quota";
            public const string PlanMetrics = Plan + "/metrics";
            public const string PlanServices = Plan + "/services";
            public const string PlanService = PlanServices + "/{1}";
            public const string PlanAddOns = Plan + "/addons";
            public const string PlanAddOn = PlanAddOns + "/{1}";
            public const string AddOn = AddOns + "/{0}";
            public const string AddOnQuota = AddOn + "/quota";
            public const string AddOnMetrics = AddOn + "/metrics";
            public const string AddOnServices = AddOn + "/services";
            public const string AddOnService = AddOnServices + "/{1}";
            public const string User = Users + "/{0}";
            public const string InvalidUserTokens = "/invalidusertokens";
            public const string Certificate = Certificates + "/{1}";
        }

                /// <summary>
        /// SPF paths class
        /// </summary>
        public static class ServiceProviderFoundation
        {
            //For Admin
            public const string VMRoleGalleryItems = "Gallery/GalleryItems/$/MicrosoftCompute.VMRoleGalleryItem";
            public const string CloudServices = "CloudServices";
            public const string CloudService = CloudServices + "/{0}";
            public const string CloudServiceComputeResources = CloudService + "/Resources/MicrosoftCompute";
            public const string CloudServiceVMRoles = CloudServiceComputeResources + "/VMRoles";
            
            //For tenant subscription
            public const string SubGalleryItems = "{0}/Gallery/GalleryItems";
            public const string SubResourceGalleryItem = "{0}/Gallery/GalleryItems/$/CloudSystem.ResourceDefinitionGalleryItem";
            public const string SubVMRoleGalleryItems = "{0}/Gallery/GalleryItems/$/MicrosoftCompute.VMRoleGalleryItem";
            public const string SubCloudServices = "{0}/CloudServices";
            public const string SubCloudService = "{0}/CloudServices/{1}";
            public const string SubCloudServiceResources = "{0}/CloudServices/{1}/Resources";
            public const string SubCloudServiceComputeResources = "{0}/CloudServices/{1}/Resources/MicrosoftCompute";
            public const string SubCloudServiceVMRoles = "{0}/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles";
            public const string SubCloudServiceVMRole = "{0}/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}";
            public const string SubCloudServiceVMRoleScale = "{0}/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/Scale";
            public const string SubCloudServiceVMRoleRepair = "{0}/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/Repair";
            public const string SubCloudServiceVMRoleVMs = "{0}/~/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/VMs";
            public const string SubCloudServiceVMRoleVM = "{0}/~/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/VMs/{3}";
            public const string SubCloudServiceVMRoleVMStop = "{0}/~/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/VMs/{3}/Stop";
            public const string SubCloudServiceVMRoleVMStart = "{0}/~/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/VMs/{3}/Start";
            public const string SubCloudServiceVMRoleVMRestart = "{0}/~/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/VMs/{3}/Restart";
            public const string SubCloudServiceVMRoleVMShutdown = "{0}/~/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/VMs/{3}/Shutdown";
            public const string SubCloudServiceVMRoleVMDisks = "{0}/~/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/VMs/{3}/Disks";
            public const string SubCloudServiceVMRoleVMDisk = "{0}/~/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/VMs/{3}/Disks/{4}";
            public const string SubCloudServiceVMRoleVMNics = "{0}/~/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/VMs/{3}/Nics";
            public const string SubCloudServiceVMRoleVMNic = "{0}/~/CloudServices/{1}/Resources/MicrosoftCompute/VMRoles/{2}/VMs/{3}/Nics/{4}";

        }

    }
}
