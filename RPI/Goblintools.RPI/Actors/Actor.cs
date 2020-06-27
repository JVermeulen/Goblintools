using Goblintools.RPI.Processing;
using System;

namespace Goblintools.RPI.Actors
{
    public abstract class Actor : Processor
    {
        public override string Category => "Actor";

        public HardwareDevice HardwareDevice { get; set; }

        public Observer<Observation> ValueChanged { get; set; }

        public Actor(string friendlyName, TimeSpan interval) : base(friendlyName, interval)
        {
            ValueChanged = new Observer<Observation>(friendlyName);
        }
    }
}
