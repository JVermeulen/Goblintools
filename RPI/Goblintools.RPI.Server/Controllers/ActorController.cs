using System;
using Microsoft.AspNetCore.Mvc;

namespace Goblintools.RPI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorController
    {
        private const string EmptyString = "EmptyString";

        private readonly RpiController Controller;

        public ActorController(RpiController controller)
        {
            Controller = controller;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var result = new
            {
                actors = new
                {
                    Controller.RedLED.LED,
                    Controller.SevenSegment.SevenSegment
                }
            };

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }

        [HttpPut("led")]
        public void PutLed([FromBody] bool value)
        {
           GetLed(value);
        }

        [HttpGet("led")]
        public ActionResult GetLed(bool? value = default)
        {
            if (value.HasValue)
                Controller.RedLED.Value = value.Value;

            var result = Controller.RedLED.LED;

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }

        [HttpPut("sevenSegment")]
        public void SetSevenSegment([FromBody] string value)
        {
            GetSevenSegment(value);
        }

        [HttpGet("sevenSegment")]
        public ActionResult GetSevenSegment(string value = EmptyString)
        {
            if (value != EmptyString)
                Controller.SevenSegment.SetValue(value);

            var result = Controller.SevenSegment.SevenSegment;

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }

        [HttpPut("oled")]
        public void SetOled([FromBody] string value)
        {
            GetOled(value);
        }

        [HttpGet("oled")]
        public ActionResult GetOled(string value = EmptyString)
        {
            if (value != EmptyString)
                Controller.OLED.SetValue(value);

            var result = Controller.OLED.Value;

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }

        [HttpGet("time")]
        public ActionResult Time()
        {
            var value = DateTime.Now.ToString("HH:mm").PadLeft(5);

            return GetSevenSegment(value);
        }
    }
}
