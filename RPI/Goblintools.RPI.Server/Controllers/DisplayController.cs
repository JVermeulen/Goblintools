using System;
using Microsoft.AspNetCore.Mvc;

namespace Goblintools.RPI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisplayController : Controller
    {
        private readonly RpiController Controller;

        private const string EmptyString = "";

        public DisplayController(RpiController controller)
        {
            Controller = controller;
        }

        [HttpPut]
        public void Put([FromBody] string value)
        {
            Get(value);
        }

        [HttpGet]
        public ActionResult Get(string value = EmptyString)
        {
            if (value != EmptyString)
                Controller.SevenSegment.SetValue(value);

            return new JsonResult(Controller.SevenSegment.SevenSegment);
        }
    }
}