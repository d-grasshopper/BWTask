using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Health3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        // GET health
        [HttpGet]
        public ActionResult<Dictionary<string, string>> Get()
        {
            return new Dictionary<string, string>(){
                { "status", "up" }
            };
        }
    }
}
