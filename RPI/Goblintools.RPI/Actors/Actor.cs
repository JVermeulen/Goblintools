using Goblintools.RPI.Processing;
using System;

namespace Goblintools.RPI.Actors
{
    public abstract class Actor : Processor, IDisposable
    {
        public Observer<Observation> ValueChanged { get; set; }

        public Actor(string friendlyName, int interval) : base(friendlyName, TimeSpan.FromSeconds(interval))
        {
            ValueChanged = new Observer<Observation>(friendlyName);
        }
    }
}
