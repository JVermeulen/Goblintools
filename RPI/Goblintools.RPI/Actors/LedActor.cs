using Goblintools.RPI.Processing;
using System;

namespace Goblintools.RPI.Actors
{
    public class LedActor : GpioActor
    {
        public Observation LED { get; private set; }

        public LedActor(string friendlyName, int pin) : base(friendlyName, pin, TimeSpan.FromSeconds(1))
        {
            ValueChanged.OnReceive.Subscribe(o => LED = o);

            LED = new Observation(Category, "Led", null, string.Empty, Code);

            HardwareDevice = new HardwareDevice
            {
                Name = Code,
                Type = "GPIO",
                Description = FriendlyName,
                Address = Pin.ToString(),
                Manufacturer = "Generic",
            };
        }
    }
}
