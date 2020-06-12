using Goblintools.RPI.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Goblintools.RPI.Actors
{
    public class LedActor : GpioActor
    {
        public Observation LED { get; private set; }

        public LedActor(string friendlyName, int pin) : base(friendlyName, pin, TimeSpan.FromSeconds(1))
        {
            ValueChanged.OnReceive.Subscribe(o => LED = o);
        }
    }
}
