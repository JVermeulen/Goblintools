using Goblintools.RPI.Processing;
using Iot.Device.Display;
using System;
using System.Device.I2c;

namespace Goblintools.RPI.Actuators
{
    public class HT16K33Display : Processor, IDisposable
    {
        //https://www.adafruit.com/product/881
        private Large4Digit7SegmentDisplay Display { get; set; }
        private I2cDevice Device { get; set; }

        public int BusId { get; set; }
        public int DeviceAddress { get; set; }
        public byte Brightness { get; set; }

        public string LastValue { get; private set; }

        public bool ShowTime { get; set; }

        public Observer<Observation> ValueChanged { get; set; }

        public HT16K33Display(string friendlyName) : base(friendlyName, TimeSpan.FromSeconds(1))
        {
            BusId = 1;
            DeviceAddress = Ht16k33.DefaultI2cAddress;
            Brightness = 1;

            ValueChanged = new Observer<Observation>(friendlyName);
        }

        public override void Start()
        {
            base.Start();

            if (Device == null)
                Device = I2cDevice.Create(new I2cConnectionSettings(BusId, DeviceAddress));

            if (Display == null)
                Display = new Large4Digit7SegmentDisplay(Device) { Brightness = Brightness };

            Display.Clear();
            Display.DisplayOn = true;
        }

        public override void Stop()
        {
            if (Display != null)
                Display.DisplayOn = false;

            Display?.Dispose();
            Display = null;

            Device?.Dispose();
            Device = null;

            base.Stop();
        }

        public override void Work(object value)
        {
            base.Work(value);

            if (value is Heartbeat heartbeat && ShowTime)
                WriteTime(DateTime.Now);
            else if (value is string text)
                WriteText(text);
        }

        private void WriteTime(DateTime time)
        {
            var value = time.ToString("HH:mm").PadLeft(5);

            WriteText(value);
        }

        private void WriteText(string value)
        {
            if (LastValue != value)
            {
                LastValue = value;

                Display?.Write(value);

                ValueChanged.Send(new Observation(FriendlyName, value, value));
            }
        }

        public new void Dispose()
        {
            Stop();

            base.Dispose();
        }
    }
}
