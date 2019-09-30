using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HealthPinger.Core;
using HealthPinger.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HealthPinger.Services
{
    public class HealthPingService : IPingHealthEndpoints
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly Dictionary<string, string> _serviceLocations;

        public HealthPingService(Dictionary<string, string> services)
        {
            _serviceLocations = services;
        }
        public async Task<Dictionary<string, int>> CheckHealthOfServices()
        {
            var results = new Dictionary<string, int>();
            foreach (var serviceLocation in _serviceLocations)
            {
                var response = await _httpClient.GetAsync($"http://{serviceLocation.Value.ToString()}/health");
                if (response.IsSuccessStatusCode)
                {
                    var content = JsonConvert.DeserializeObject<StatusResponse>(await response.Content.ReadAsStringAsync());
                    if (content.Status == "up") results.Add(serviceLocation.Key, 1);
                }
            }
            return results;
        }
    }
}
