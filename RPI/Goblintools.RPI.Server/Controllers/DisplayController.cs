using System;
using Goblintools.RPI.Actuators;
using Microsoft.AspNetCore.Mvc;

namespace Goblintools.RPI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisplayController : Controller
    {
        private readonly RpiController Controller;

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
        public ActionResult Get(string value = default)
        {
            return new JsonResult(null);
        }
    }
}