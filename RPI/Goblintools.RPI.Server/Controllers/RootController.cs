using System;
using System.Linq;
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
        public ActionResult Get(string search = null)
        {
            var result = Controller.SearchObservations(search);

            return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
        }
    }
}