using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using GistackWAPService.models;
using System.Runtime.Serialization;

namespace GistackWAPService.models
{
    /// <summary>
    /// Model class to hold Plan detail
    /// </summary>
     [DataContract]
    public class Plan
    {
         [DataMember]
        public string PlanId { get; set; }
         [DataMember]
        public string PlanDisplayName { get; set; }
         [DataMember]
        public string PlanPrice { get; set; }
         [DataMember]
        public int State { get; set; }
         [DataMember]
        public int ConfigState { get; set; }
     }

    /// <summary>
    /// Model class to hold Plan list
    /// </summary>
    public class PlanList : List<Plan>
    {
        /// <summary>
        /// Initializes a new instance of the Plan class.
        /// </summary>
        public PlanList()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plan"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        public PlanList(IEnumerable<Plan> records)
            : base(records)
        {
        }
    }
}