using Goblintools.RPI.Actors;
using Goblintools.RPI.Processing;
using Goblintools.RPI.Sensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Goblintools.RPI
{
    public class RpiController : IDisposable
    {
        public GpioActor LED { get; set; }
        public Bme280Sensor BME280 { get; set; }

        public RpiController()
        {
            LED = new GpioActor("Led", 24);
            LED.Start();

            BME280 = new Bme280Sensor("Temperature, Humidity and Pressure Sensor");
            BME280.Start();
        }

        public void Dispose()
        {
            LED?.Dispose();
            BME280?.Dispose();
        }
    }
}
