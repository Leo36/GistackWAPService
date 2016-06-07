using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using GistackWAPService.models;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace GistackWAPService.models
{
    /// <summary>
    /// Model class to hold VM detail
    /// </summary>
      [DataContract]
    public class VirtualMachine
    {

         [DataMember]
          public Guid id { get; set; }
         [DataMember]
          public string name { get; set; }
        [DataMember]
          public string computerName { get; set; }
        [DataMember]
          public string virtualMachineStatus { get; set; }
         [DataMember]
          public int cpuCount { get; set; }
        [DataMember]
          public int memoryAssignedMB { get; set; }
          [DataMember]
          public int dynamicMemoryMinimumMB { get; set; }

         [DataMember]
          public int perfCPUUtilization { get; set; }
        [DataMember]
          public int perfMemory { get; set; }

         [DataMember]
          public string cloud { get; set; }
        [DataMember]
          public string vmTemplate { get; set; }
          [DataMember]
          public string operatingSystem{ get; set; }
        [DataMember]
         public ObservableCollection<string> ips { get; set; }
        [DataMember]
          public IEnumerable<String> virtualHardDisks { get; set; }
        [DataMember]
          public IEnumerable<String> virtualNetworkAdapters { get; set; }

     }

    /// <summary>
    /// Model class to hold VM List
    /// </summary>
    public class VirtualMachineList : List<VirtualMachine>
    {
        /// <summary>
        /// Initializes a new instance of the Virtual Machine class.
        /// </summary>
        public VirtualMachineList()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualMachineList"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        public VirtualMachineList(IEnumerable<VirtualMachine> records)
            : base(records)
        {
        }
    }
}