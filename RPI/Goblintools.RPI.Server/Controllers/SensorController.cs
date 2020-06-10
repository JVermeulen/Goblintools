using System;
using Goblintools.RPI.Sensors;
using Iot.Units;
using Microsoft.AspNetCore.Mvc;

namespace Goblintools.RPI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensorController
    {
        private readonly RpiController Controller;

        public SensorController(RpiController controller)
        {
            Controller = controller;
        }

        [HttpGet]
        public ActionResult Get()
        {
            //BME280.Read(out Temperature temperature, out Pressure pressure, out double humidity);

            //var model = new
            //{
            //    temperature = Math.Round(temperature.Celsius, 3),
            //    pressure = Math.Round(pressure.Hectopascal, 3),
            //    humidity = Math.Round(humidity, 3),
            //};

            //return new JsonResult(model);

            return new JsonResult(null);
        }
    }
}
