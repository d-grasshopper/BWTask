using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace HealthPinger
{
    public static class Function1
    {
        [FunctionName("CheckHealth")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"CheckHealth function executed at: {DateTime.Now}");
        }
    }
}
