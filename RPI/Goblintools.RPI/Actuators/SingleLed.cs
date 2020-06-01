using Goblintools.RPI.Processing;
using System;
using System.Device.Gpio;

namespace Goblintools.RPI.Actuators
{
    public class SingleLed : Processor, IDisposable
    {
        private GpioController Controller { get; set; }

        public int Pin { get; set; }

        public bool BlinkLed { get; set; }

        public bool LastValue { get; private set; }

        public Observer<ActuatorValueChanged> ValueChanged { get; set; }

        public SingleLed(int pin, int interval = 1) : base(TimeSpan.FromSeconds(interval))
        {
            Pin = pin;

            ValueChanged = new Observer<ActuatorValueChanged>("LED");
        }

        public override void Start()
        {
            Console.WriteLine($"{Name} started.");

            if (Controller == null)
            {
                Controller = new GpioController();

                Controller.OpenPin(Pin, PinMode.Output);

                Console.WriteLine($"  Setting LED pin to {Pin}.");
            }

            base.Start();
        }

        public override void Stop()
        {
            Console.WriteLine($"{Name} stopped.");

            Controller?.Dispose();
            Controller = null;

            base.Stop();
        }

        public override void Work(object value)
        {
            base.Work(value);

            if (value is Heartbeat heartbeat && BlinkLed)
                SetLed(!LastValue);
            else if (value is bool turnOn)
                SetLed(turnOn);
        }

        private void SetLed(bool turnOn)
        {
            if (LastValue != turnOn)
            {
                LastValue = turnOn;

                Controller.Write(Pin, turnOn ? PinValue.High : PinValue.Low);

                ValueChanged.Send(new ActuatorValueChanged(ValueChanged.Name, turnOn));
            }
        }

        public new void Dispose()
        {
            Stop();

            base.Dispose();
        }
    }
}
