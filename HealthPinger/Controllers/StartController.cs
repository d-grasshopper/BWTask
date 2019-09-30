using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthPinger.Core;
using Microsoft.AspNetCore.Mvc;

namespace HealthPinger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StartController : ControllerBase
    {
        private readonly ICheckHealthAndPost _healthChecker;
        public StartController(ICheckHealthAndPost healthChecker)
        {
            _healthChecker = healthChecker;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            ThreadPool.QueueUserWorkItem(state => _healthChecker.StartChecking());

            return Ok("Checking has started");
        }
    }
}
