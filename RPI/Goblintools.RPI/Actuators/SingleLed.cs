using Goblintools.RPI.Processing;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;
using System.Threading.Tasks;

namespace Goblintools.RPI.Actuators
{
    public class SingleLed : Processor, IDisposable
    {
        private GpioController Controller { get; set; }

        public int Pin { get; set; }

        public bool BlinkLed { get; set; }

        public bool LastValue { get; private set; }

        public Observer<object> ValueChanged { get; set; }

        public SingleLed(int pin, int interval = 1) : base(TimeSpan.FromSeconds(interval))
        {
            Pin = pin;

            ValueChanged = new Observer<object>("LED");
        }

        public override void Start()
        {
            if (Controller == null)
            {
                Controller = new GpioController();

                Controller.OpenPin(Pin, PinMode.Output);
            }

            base.Start();
        }

        public override void Stop()
        {
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
            else if (value is string text)
                Blink(50);
        }

        private void Blink(int delay)
        {
            SetLed(!LastValue);

            Task.Delay(100);
            
            SetLed(!LastValue);
        }

        private void SetLed(bool turnOn)
        {
            if (LastValue != turnOn)
            {
                LastValue = turnOn;

                Controller.Write(Pin, turnOn ? PinValue.High : PinValue.Low);

                ValueChanged.Send(turnOn);
            }
        }

        public new void Dispose()
        {
            Stop();

            base.Dispose();
        }
    }
}
