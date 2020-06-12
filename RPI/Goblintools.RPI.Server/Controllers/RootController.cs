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
                sensors = new
                {
                    Controller.BME280.Temperature,
                    Controller.BME280.Pressure,
                    Controller.BME280.Humidity,
                    Controller.BME280.Altitude,
                },
                actors = new
                {
                    Controller.RedLED.LED,
                    Controller.SevenSegment.SevenSegment
                },
            };

            return new JsonResult(result);
        }
    }
}