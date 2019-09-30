using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthPinger.Core
{
    public interface ICheckHealthAndPost
    {
        void StartChecking();
    }
}
