using System;
using System.Device.I2c;

namespace Goblintools.RPI.Actors
{
    public class I2cActor : Actor
    {
        protected I2cConnectionSettings Settings { get; set; }
        protected I2cDevice Device { get; set; }

        protected int BusId { get; set; }
        protected int DeviceAddress { get; set; }

        public I2cActor(string friendlyName, int deviceAddress, TimeSpan interval) : base(friendlyName, interval)
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
