using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Text;

namespace Goblintools.RPI.Sensors
{
    public class I2cSensor : Sensor
    {
        protected I2cConnectionSettings Settings { get; set; }
        protected I2cDevice Device { get; set; }

        protected int BusId { get; set; }
        protected int DeviceAddress { get; set; }

        public I2cSensor(string friendlyName, int deviceAddress) : base(friendlyName, 15)
        {
            try
            {
                BusId = 1;
                DeviceAddress = deviceAddress;

                Settings = new I2cConnectionSettings(BusId, DeviceAddress);
                Device = I2cDevice.Create(Settings);
            }
            catch
            {
                //
            }
        }

        public new void Dispose()
        {
            base.Dispose();

            Device?.Dispose();
            Device = null;

            Settings = null;
        }
    }
}
