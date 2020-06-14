using Goblintools.RPI.Processing;
using System;
using System.Device.Gpio;

namespace Goblintools.RPI.Actors
{
    public class GpioActor : Actor
    {
        private GpioController Controller { get; set; }

        public int Pin { get; private set; }

        public override string Code => "GPIO";

        public bool Value
        {
            get
            {
                return Controller != null ? Controller.Read(Pin) == PinValue.High : false;
            }
            set
            {
                Controller?.Write(Pin, value ? PinValue.High : PinValue.Low);

                Read();
            }
        }

        public GpioActor(string friendlyName, int pin, TimeSpan interval) : base(friendlyName, interval)
        {
            Pin = pin;
        }

        public override void Start()
        {
            base.Start();

            try
            {
                if (Controller == null)
                {
                    Controller = new GpioController();

                    Controller.OpenPin(Pin, PinMode.Output);

                    Read();
                }
            }
            catch (NotSupportedException ex)
            {
                Controller = null;
            }
        }

        public override void Stop()
        {
            base.Stop();

            if (Controller != null && Controller.IsPinOpen(Pin))
                Controller.ClosePin(Pin);
        }

        public void Read()
        {
            var observation = new Observation(Category, "Led", Value, Value ? "On" : "Off", Code);

            ValueChanged.Send(observation);
        }

        public new void Dispose()
        {
            Stop();

            base.Dispose();
        }
    }
}
