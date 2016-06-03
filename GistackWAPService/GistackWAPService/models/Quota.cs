using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using GistackWAPService.models;
using System.Runtime.Serialization;

namespace GistackWAPService.models
{
    /// <summary>
    /// Model class to hold VM detail
    /// </summary>
    [DataContract]
    public class Quota
    {

         [DataMember]
        public string displayName { get; set; }
         [DataMember]
        public string groupDisplayName { get; set; }
         [DataMember]
        public string groupId { get; set; }
        [DataMember]
        public string unitDisplayName { get; set; }

        [DataMember]
        public long limit { get; set; }
         [DataMember]
        public string limitString { get; set; }
         [DataMember]
        public long currentValue { get; set; }
     }

    /// <summary>
    /// Model class to hold VM List
    /// </summary>
    public class QuotaList : List<Quota>
    {
        /// <summary>
        /// Initializes a new instance of the Virtual Machine class.
        /// </summary>
        public QuotaList()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualMachineList"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        public QuotaList(IEnumerable<Quota> records)
            : base(records)
        {
        }
    }
}