using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Goblintools.RPI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController
    {
        private readonly RpiController Controller;

        public TestController(RpiController controller)
        {
            Controller = controller;
        }

        [HttpGet]
        public ActionResult Get()
        {
            //var result = Controller.I2cDetect().Select((v, i) => new
            //{
            //    address = $"0x{i.ToString("X2")}",
            //    isActive = v,
            //});

            var result = new 
            { 
                active = Controller.I2cDetect().Select(i => $"0x{i.ToString("X2")}").ToArray()
            };

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }
    }
}
