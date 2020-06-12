using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Goblintools.RPI.Blazor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GpioController : ControllerBase
    {
        private readonly RpiProcessor RPI;

        public GpioController(RpiProcessor rpi)
        {
            RPI = rpi;

            if (!rpi.IsRunning)
                rpi.Start();
        }

        [HttpGet]
        public ActionResult Get()
        {
            var model = new
            {
                values = RPI.RotateText.Select(kvp => new { kvp.Key, kvp.Value}).ToArray(),
            };

            return new JsonResult(model);
        }

        [HttpPost]
        public void Post([FromBody] bool toggle)
        {
            //if (toggle)
            //    RPI.LED.Inbox.Send(SingleLedMode.On);
            //else
            //    RPI.LED.Inbox.Send(SingleLedMode.Off);
        }
    }
}