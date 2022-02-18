using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        async Task<string> AsyncTest()
        {
            await Task.Yield();

            return "Hi from AsyncTest()";
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var r = this.AsyncTest().Result;

            return r;
        }
    }
}
