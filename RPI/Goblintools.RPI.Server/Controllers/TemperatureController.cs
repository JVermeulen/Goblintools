using System;
using Microsoft.AspNetCore.Mvc;

namespace Goblintools.RPI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemperatureController
    {
        private readonly RpiController Controller;

        public TemperatureController(RpiController controller)
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
                }
            };

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }
    }
}
