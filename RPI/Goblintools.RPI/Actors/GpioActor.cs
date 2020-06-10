using System;
using System.Device.Gpio;

namespace Goblintools.RPI.Actors
{
    public class GpioActor : Actor
    {
        private GpioController Controller { get; set; }

        public int Pin { get; private set; }

        public bool Value
        {
            get
            {
                return Controller != null ? Controller.Read(Pin) == PinValue.High : false;
            }
            set
            {
                Controller?.Write(Pin, value ? PinValue.High : PinValue.Low);
            }
        }

        public GpioActor(string friendlyName, int pin, int interval = 15) : base(friendlyName, interval)
        {
            Pin = pin;
        }

        public override void Start()
        {
            base.Start();

            if (Controller == null)
            {
                Controller = new GpioController();

                Controller.OpenPin(Pin, PinMode.Output);
            }
        }

        public override void Stop()
        {
            base.Stop();

            if (Controller != null && Controller.IsPinOpen(Pin))
                Controller.ClosePin(Pin);
        }

        public new void Dispose()
        {
            Stop();

            base.Dispose();
        }
    }
}
