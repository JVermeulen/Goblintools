using Iot.Device.Display;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Text;

namespace Goblintools.RPI.Actuators
{
    //https://www.adafruit.com/product/881
    public class SevenSegmentDisplay : IDisposable
    {
        private int BusId { get; set; }
        private int DeviceAddress { get; set; }
        private byte Brightness { get; set; }
        private I2cDevice Device { get; set; }
        private Large4Digit7SegmentDisplay Display { get; set; }

        private string _Value;
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                Display.Write(value);
                Display.DisplayOn = value != null;
                _Value = value;
            }
        }

        public SevenSegmentDisplay()
        {
            BusId = 1;
            DeviceAddress = Ht16k33.DefaultI2cAddress;
            Brightness = 1;

            if (Device == null)
                Device = I2cDevice.Create(new I2cConnectionSettings(BusId, DeviceAddress));

            if (Display == null)
                Display = new Large4Digit7SegmentDisplay(Device) { Brightness = Brightness };
        }

        public void Dispose()
        {
            Display?.Dispose();
            Display = null;

            Device?.Dispose();
            Device = null;
        }
    }
}
