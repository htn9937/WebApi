using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BookManagementAPI.ErrorHelper
{
    [Serializable]
    [DataContract]
    public class ServiceStatus
    {

        [JsonProperty("StatusMessage")]
        [DataMember]
        public string StatusMessage { get; set; }

        [JsonProperty("ReasonPhrase")]
        [DataMember]
        public string ReasonPhrase { get; set; }
    }
}