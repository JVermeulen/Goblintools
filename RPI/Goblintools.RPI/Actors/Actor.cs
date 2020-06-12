using Goblintools.RPI.Processing;
using System;

namespace Goblintools.RPI.Actors
{
    public abstract class Actor : Processor, IDisposable
    {
        public Observer<Observation> ValueChanged { get; set; }

        public Actor(string friendlyName, TimeSpan interval) : base(friendlyName, interval)
        {
            ValueChanged = new Observer<Observation>(friendlyName);
        }
    }
}
