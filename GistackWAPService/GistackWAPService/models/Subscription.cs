using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using GistackWAPService.models;
using System.Runtime.Serialization;

namespace GistackWAPService.models
{
    /// <summary>
    /// Model class to hold subscription detail
    /// </summary>
   [DataContract]
    public class Subscription
    {
        [DataMember] 
       public string subscriptionID { get; set; }
        [DataMember] 
       public string name { get; set; }
        [DataMember] 
       public string status { get; set; }
        [DataMember]
       public string offerCategory { get; set; }
        [DataMember] 
       public string offerFriendlyName { get; set; }
        [DataMember] 
       public string planId { get; set; }
        [DataMember]
       public string planDisplayName { get; set; }
        [DataMember]
       public string registeredServices { get; set; }
     }

    /// <summary>
    /// Model class to hold subscription list
    /// </summary>
    public class SubscriptionList : List<Subscription>
    {
        /// <summary>
        /// Initializes a new instance of the Subscription class.
        /// </summary>
        public SubscriptionList()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        public SubscriptionList(IEnumerable<Subscription> records)
            : base(records)
        {
        }
    }
}