using Goblintools.RPI.Processing;
using Iot.Device.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Goblintools.RPI.Actors
{
    public class SevenSegmentActor : I2cActor
    {
        public override string Code => "HT16K33";

        public Observation SevenSegment { get; private set; }
        private Large4Digit7SegmentDisplay Display { get; set; }
        private byte Brightness { get; set; }

        public Dictionary<string, string> Values { get; set; }
        public int Index { get; set; }

        public SevenSegmentActor(string friendlyName) : base(friendlyName, Ht16k33.DefaultI2cAddress, TimeSpan.FromSeconds(1))
        {
            Values = new Dictionary<string, string>();

            SevenSegment = new Observation(Category, FriendlyName, null, string.Empty, Code);

            ValueChanged.OnReceive.Subscribe(o => SevenSegment = o);

            Brightness = 1;

            if (Device != null)
                Display = new Large4Digit7SegmentDisplay(Device) { Brightness = Brightness };

            SetValue(null);

            HardwareDevice = new HardwareDevice
            {
                Name = Code,
                Description = FriendlyName,
                Type = "I2C",
                Address = $"0x{Ht16k33.DefaultI2cAddress.ToString("X2")}",
                Manufacturer = "Adafruit",
                Reference = "https://www.adafruit.com/product/881",
            };
        }

        public override void Work(object value)
        {
            try
            {
                if (value is Heartbeat heartbeat)
                    OnHeartbeat(heartbeat);
            }
            catch (Exception ex)
            {
                WriteToConsole(ex.Message, ConsoleColor.Red);
            }
        }

        public void OnHeartbeat(Heartbeat heartbeat)
        {
            SetValue("Time", DateTime.Now.ToString("mm:ss").PadLeft(5));

            if (Index + 1 < Values.Count)
                Index++;
            else
                Index = 0;

            //SetValue(Values.ElementAt(Index).Value);
        }

        public void SetValue(string key, string value)
        {
            if (Values.ContainsKey(key))
                Values[key] = value;
            else
                Values.Add(key, value);
        }

        public void SetValue(DateTime value, string format = "HH:mm")
        {
            SetValue(value.ToString(format).PadLeft(5));
        }

        public void SetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                value = null;

            bool hasChanged = (SevenSegment == null || SevenSegment.Value == null || value == null) || !((string)SevenSegment.Value).Equals(value);

            if (Display != null && hasChanged)
            {
                if (value != null)
                {
                    Display.Write(value);

                    if (!Display.DisplayOn)
                        Display.DisplayOn = true;
                }
                else
                {
                    Display.Clear();

                    if (Display.DisplayOn)
                        Display.DisplayOn = false;
                }

                SevenSegment = new Observation(Category, "SevenSegment", value, value, Code);

                ValueChanged.Send(SevenSegment);
            }
        }

        public new void Dispose()
        {
            SetValue(null);

            base.Dispose();

            Display?.Dispose();
            Display = null;
        }
    }
}
