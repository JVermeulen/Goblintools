using System;
using Microsoft.AspNetCore.Mvc;

namespace Goblintools.RPI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LedController : ControllerBase
    {
        private readonly RpiController Controller;

        public LedController(RpiController controller)
        {
            Controller = controller;
        }

        [HttpPut]
        public void Put([FromBody] bool value)
        {
            Get(value);
        }

        [HttpGet]
        public ActionResult Get(bool? value = default)
        {
            if (value.HasValue)
                Controller.RedLED.Value = value.Value;

            var result = new
            {
                led = Controller.RedLED.LED,
            };

            return new JsonResult(result);
        }
    }
}
