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
        public ActionResult Get(string keyword = null)
        {
            var observations = Controller.GetObservations();

            if (keyword == null)
            {
                return new JsonResult(observations, Controller.DefaultJsonSerializerOptions);
            }
            else
            {
                var result = observations.Where(o => new string[]
                {
                    o.Category.ToLower(),
                    o.MachineName.ToLower(),
                    o.DeviceName.ToLower(),
                    o.Name.ToLower()
                }.Contains(keyword.ToLower()));

                return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
            }
        }
    }
}