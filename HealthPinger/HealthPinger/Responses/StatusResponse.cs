using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HealthPinger.Responses
{
    public class StatusResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
