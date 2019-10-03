using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthPinger.Services
{
    public interface IPostToElastic
    {
        Task<bool> PostToElastic(IEnumerable<KeyValuePair<string, int>> results);
    }
}
