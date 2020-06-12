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
        //https://www.adafruit.com/product/881

        public override string Code => "HT16K33";

        public Observation SevenSegment { get; private set; }
        private Large4Digit7SegmentDisplay Display { get; set; }
        private byte Brightness { get; set; }

        public Dictionary<string, string> Values { get; set; }
        public int Index { get; set; }

        public SevenSegmentActor(string friendlyName) : base(friendlyName, Ht16k33.DefaultI2cAddress, TimeSpan.FromSeconds(1))
        {
            Values = new Dictionary<string, string>();

            ValueChanged.OnReceive.Subscribe(o => SevenSegment = o);

            Brightness = 1;

            Display = new Large4Digit7SegmentDisplay(Device) { Brightness = Brightness };

            SetValue(null);
        }

        public override void Work(object value)
        {
            if (value is Heartbeat heartbeat)
                OnHeartbeat(heartbeat);
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
            bool hasChanged = (SevenSegment == null || SevenSegment.Value == null || value == null) || !((string)SevenSegment.Value).Equals(value);

            if (hasChanged)
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

                SevenSegment = new Observation(false, "SevenSegment", value, value, Code);

                ValueChanged.Send(SevenSegment);
            }
        }

        public new void Dispose()
        {
            base.Dispose();

            SetValue(null);

            Display?.Dispose();
            Display = null;
        }
    }
}
