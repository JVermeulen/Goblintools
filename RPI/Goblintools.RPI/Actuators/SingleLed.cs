using Goblintools.RPI.Processing;
using System;
using System.Device.Gpio;

namespace Goblintools.RPI.Actuators
{
    public class SingleLed : Processor, IDisposable
    {
        private GpioController Controller { get; set; }

        public byte? RedPin { get; private set; }
        public byte? GreenPin { get; private set; }
        public byte? BluePin { get; private set; }

        public Color Value { get; private set; }
        public Color ValueInversed => Value.CreateInversed();

        public SingleLedMode Mode { get; set; }

        public Observer<ActuatorValueChanged> ValueChanged { get; set; }

        public SingleLed(string friendlyName, byte? redPin = null, byte? greenPin = null, byte? bluePin = null, int interval = 1) : base(friendlyName, TimeSpan.FromSeconds(interval))
        {
            RedPin = redPin;
            GreenPin = greenPin;
            BluePin = bluePin;

            Value = new Color(byte.MaxValue);

            ValueChanged = new Observer<ActuatorValueChanged>(friendlyName);
        }

        public override void Start()
        {
            base.Start();

            if (Controller == null)
            {
                Controller = new GpioController();
                
                if (RedPin.HasValue)
                    Controller.OpenPin(RedPin.Value, PinMode.Output);

                if (GreenPin.HasValue)
                    Controller.OpenPin(GreenPin.Value, PinMode.Output);

                if (BluePin.HasValue)
                    Controller.OpenPin(BluePin.Value, PinMode.Output);

                WriteToConsole($"  Setting GPIO pins of {FriendlyName} to red={RedPin}, green={GreenPin}, blue={BluePin}.", ConsoleColor.Yellow);
            }
        }

        public override void Stop()
        {
            base.Stop();

            if (Controller != null)
            {
                if (RedPin.HasValue)
                    Controller.ClosePin(RedPin.Value);

                if (GreenPin.HasValue)
                    Controller.ClosePin(GreenPin.Value);

                if (BluePin.HasValue)
                    Controller.ClosePin(BluePin.Value);

                Controller.Dispose();
                Controller = null;
            }
        }

        public override void Work(object value)
        {
            base.Work(value);

            if (value is Heartbeat heartbeat)
                OnHeartbeat(heartbeat);
            else if (value is SingleLedMode mode)
                SetMode(mode);
        }

        private void OnHeartbeat(Heartbeat heartbeat)
        {
            if (Mode == SingleLedMode.Blink)
                SetColor(ValueInversed);
        }

        private void SetMode(SingleLedMode mode)
        {
            Mode = mode;

            if (Mode == SingleLedMode.On)
                SetColor(new Color(byte.MaxValue));
            else if (mode == SingleLedMode.Off)
                SetColor(new Color(byte.MinValue));
        }

        private void SetColor(Color value)
        {
            if (Value != value)
            {
                Value = value;

                if (RedPin.HasValue)
                    Controller.Write(RedPin.Value, value.Brightness > 0 && Value.Red > 0 ? PinValue.High : PinValue.Low);

                if (GreenPin.HasValue)
                    Controller.Write(GreenPin.Value, value.Brightness > 0 && Value.Green > 0 ? PinValue.High : PinValue.Low);

                if (BluePin.HasValue)
                    Controller.Write(BluePin.Value, value.Brightness > 0 && Value.Blue > 0 ? PinValue.High : PinValue.Low);
                
                ValueChanged.Send(new ActuatorValueChanged(FriendlyName, value, value.Brightness > 0 ? "on" : "off"));
            }
        }

        public new void Dispose()
        {
            Stop();

            base.Dispose();
        }
    }
}
