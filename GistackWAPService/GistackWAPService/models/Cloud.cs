using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GistackWAPService.models
{
    /// <summary>
    /// Model class to hold cloud detail
    /// </summary>
   [DataContract]
    public class Cloud
    {
       [DataMember]
       public Guid id { get; set; }
       [DataMember]
       public string name { get; set; }
    }

    /// <summary>
    /// Model class to hold list of cloud
    /// </summary>
    public class CloudList : List<Cloud>
    {
        /// <summary>
        /// Initializes a new instance of the Cloud class.
        /// </summary>
        public CloudList()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Cloud class.
        /// </summary>
        /// <param name="records">The records.</param>
        public CloudList(IEnumerable<Cloud> records)
            : base(records)
        {
        }
    }
}