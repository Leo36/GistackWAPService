using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GistackWAPService.models
{
    [DataContract]
    public class Result
    {
        [DataMember]
        public string result { get; set; }
        [DataMember]
        public string message { get; set; }

        public Result(string result, string message)
        {
            this.result = result;
            this.message = message;
        }
    }
}