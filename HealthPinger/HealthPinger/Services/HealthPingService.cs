using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HealthPinger.Responses;
using Newtonsoft.Json;

namespace HealthPinger.Services
{
    public class HealthPingService : IPingHealthEndpoints
    {
        private readonly IHttpWrapper _httpClient;
        private readonly Dictionary<string, string> _serviceLocations;

        public HealthPingService(IHttpWrapper httpClient, Dictionary<string, string> services)
        {
            _httpClient = httpClient;
            _serviceLocations = services;
        }
        public async Task<Dictionary<string, int>> CheckHealthOfServices()
        {
            var results = new Dictionary<string, int>();
            foreach (var serviceLocation in _serviceLocations)
            {
                var response = await _httpClient.GetAsync($"http://{serviceLocation.Value.ToString()}/health");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = JsonConvert.DeserializeObject<StatusResponse>(await response.Content.ReadAsStringAsync());
                    if (content.Status == "up") results.Add(serviceLocation.Key, 1);
                }
                else
                {
                    results.Add(serviceLocation.Key, 0);
                }
            }
            return results;
        }
    }
}
