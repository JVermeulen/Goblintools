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

                //var result = new
                //{
                //    sensors = new
                //    {
                //        Controller.BME280.Temperature,
                //        Controller.BME280.Pressure,
                //        Controller.BME280.Humidity,
                //    },
                //    actors = new
                //    {
                //        Controller.RedLED.LED,
                //        Controller.SevenSegment.SevenSegment
                //    },
                //};

                //return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
            }
            else
            {
                var result = observations.Where(o => o.Keywords.Contains(keyword.ToLower()));

                return new JsonResult(result, Controller.DefaultJsonSerializerOptions);
            }
        }
    }
}