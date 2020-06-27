using Goblintools.RPI.Processing;
using System;

namespace Goblintools.RPI.Sensors
{
    public abstract class Sensor : Processor, IDisposable
    {
        public override string Category => "Sensor";

        public HardwareDevice HardwareDevice { get; set; }

        public Observer<Observation> ValueChanged { get; set; }

        public Sensor(string friendlyName, int interval) : base(friendlyName, TimeSpan.FromSeconds(interval))
        {
            ValueChanged = new Observer<Observation>(friendlyName);
        }
    }
}
