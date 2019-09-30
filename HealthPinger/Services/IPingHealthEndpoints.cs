using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthPinger.Services
{
    public interface IPingHealthEndpoints
    {
        Task<Dictionary<string, int>> CheckHealthOfServices();
    }
}
