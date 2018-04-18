using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [Serializable]
    [DataContract]
    public class ApiDataException : Exception
    {
        [DataMember]
        public string ErrorDescription { get; set; }
        [DataMember]
        public HttpStatusCode HttpStatus { get; set; }

        string reasonPhrase = "ApiDataException";

        [DataMember]
        public string ReasonPhrase
        {
            get { return this.reasonPhrase; }

            set { this.reasonPhrase = value; }
        }

        public ApiDataException(string errorDescription, HttpStatusCode httpStatus)
        {
            ErrorDescription = errorDescription;
            HttpStatus = httpStatus;
        }
    }
}
