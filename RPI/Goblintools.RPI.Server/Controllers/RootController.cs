using System;
using Microsoft.AspNetCore.Mvc;

namespace Goblintools.RPI.Server.Controllers
{
    [ApiController]
    [Route("")]
    public class RootController
    {
        private readonly RpiController Controller;

        public RootController(RpiController controller)
        {
            Controller = controller;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var result = new
            {
                temperature = Controller.BME280.Temperature,
                pressure = Controller.BME280.Pressure,
                humidity = Controller.BME280.Humidity,
                led = Controller.LED.Value,
            };

            return new JsonResult(result);
        }

    }
}
