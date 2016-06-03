using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GistackWAPService.models
{
    /// <summary>
    /// Model class to hold VMTemplate detail
    /// </summary>
     [DataContract]
    public class VMTemplate
    {
         [DataMember]
         public Guid id { get; set; }
          [DataMember]
         public string name { get; set; }
    }

    /// <summary>
    /// Model class to hold VMTemplate List
    /// </summary>
    public class VMTemplateList : List<VMTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VMTemplateList"/> class.
        /// </summary>
        public VMTemplateList()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the VMTemplate class.
        /// </summary>
        /// <param name="records">The records.</param>
        public VMTemplateList(IEnumerable<VMTemplate> records)
            : base(records)
        {
        }
    }
}