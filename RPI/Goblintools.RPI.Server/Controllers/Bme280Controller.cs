using System;
using Microsoft.AspNetCore.Mvc;

namespace Goblintools.RPI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Bme280Controller
    {
        private readonly RpiController Controller;

        public Bme280Controller(RpiController controller)
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
                }
            };

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }
    }
}
