using System;
using System.Text.Json;
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
            var result = new
            {
                sensors = new
                {
                    Controller.BME280.Temperature,
                    Controller.BME280.Pressure,
                    Controller.BME280.Humidity,
                    Controller.VCNL4000.AmbientLight,
                }
            };

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }

        [HttpGet("temperature")]
        public ActionResult Temperature(string format = "JSON")
        {
            var result = Controller.BME280.Temperature;
            var value = result?.Value != null ? (double)result.Value : 20;

            switch (format)
            {
                case "string":
                    return new ContentResult { Content = value.ToString("F2") };
                default:
                    return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
            }
        }

        [HttpGet("pressure")]
        public ActionResult Pressure()
        {
            var result = Controller.BME280.Pressure;

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }

        [HttpGet("humidity")]
        public ActionResult Humidity()
        {
            var result = Controller.BME280.Humidity;

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }
    }
}
