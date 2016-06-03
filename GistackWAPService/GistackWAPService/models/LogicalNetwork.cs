using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GistackWAPService.models
{
     [DataContract]
    public class LogicalNetwork
    {
        [DataMember]
        public Guid id { get; set; }
        [DataMember]
        public string name { get; set; }
    }
}