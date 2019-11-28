using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Common.Models
{
    public class SDKModel
    {
        [JsonConverter(typeof(MemoryStreamJsonConverter))]
        public MemoryStream Body { get; set; }
        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
    }
}
