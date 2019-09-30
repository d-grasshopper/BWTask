using System;
using System.Threading;
using System.Threading.Tasks;
using HealthPinger.Services;

namespace HealthPinger.Core
{
    public class HealthChecker : ICheckHealthAndPost
    {
        private readonly IPingHealthEndpoints _healthPingService;
        private readonly IPostToElastic _elasticService;

        public HealthChecker(IPingHealthEndpoints healthPingService, IPostToElastic elasticService)
        {
            _healthPingService = healthPingService;
            _elasticService = elasticService;
        }
        public void StartChecking()
        {
            var timer = new Timer(async (state) => await CheckServicesAndPersist(), null, 5000, 5000);
        }

        public async Task CheckServicesAndPersist()
        {
            var result = await _healthPingService.CheckHealthOfServices();
            foreach (var service in result)
            {
                Console.WriteLine("Service: " + result.ToString());
            }

            var elasticResult = await _elasticService.PostToElastic(result);
            Console.WriteLine("Elastic result: " + elasticResult);
        }
    }
}
