using Goblintools.RPI.Processing;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.FilteringMode;
using Iot.Device.Bmxx80.PowerMode;
using System;
using System.Device.I2c;
using System.Threading;

namespace Goblintools.RPI.Sensors
{
    public class Bme280Sensor : Processor, IDisposable
    {
        //https://www.adafruit.com/product/2652
        private Bme280 Sensor { get; set; }
        private I2cConnectionSettings Settings { get; set; }
        private I2cDevice Device { get; set; }

        public int BusId { get; set; }
        public int DeviceAddress { get; set; }

        public Observer<object> ValueChanged { get; set; }

        public Bme280Sensor(string friendlyName, int interval = 15) : base(friendlyName, TimeSpan.FromSeconds(interval))
        {
            BusId = 1;
            DeviceAddress = Bme280.DefaultI2cAddress;

            ValueChanged = new Observer<object>(friendlyName);
        }

        public override void Start()
        {
            if (Settings == null)
                Settings = new I2cConnectionSettings(BusId, DeviceAddress);

            if (Device == null)
                Device = I2cDevice.Create(Settings);

            if (Sensor == null)
            {
                Sensor = new Bme280(Device);
                Sensor.SetPowerMode(Bmx280PowerMode.Forced);
            }

            base.Start();
        }

        public override void Stop()
        {
            Sensor?.Dispose();
            Sensor = null;

            Device?.Dispose();
            Device = null;

            Settings = null;

            base.Stop();
        }

        public override void Work(object value)
        {
            if (value is Heartbeat heartbeat)
                Read();
        }

        public void Read()
        {
            if (!base.IsEnabled)
                throw new ApplicationException("Unable to read. Sensor has not started yet.");

            // set higher sampling
            Sensor.TemperatureSampling = Sampling.LowPower;
            Sensor.PressureSampling = Sampling.UltraHighResolution;
            Sensor.HumiditySampling = Sampling.Standard;

            // set mode forced so device sleeps after read
            Sensor.SetPowerMode(Bmx280PowerMode.Forced);

            // wait for measurement to be performed
            var measurementTime = Sensor.GetMeasurementDuration();
            Thread.Sleep(measurementTime);

            if (Sensor.TryReadTemperature(out var temperature))
                ValueChanged.Send(temperature);

            if (Sensor.TryReadPressure(out var pressure))
                ValueChanged.Send(pressure);

            if (Sensor.TryReadHumidity(out var humidity))
                ValueChanged.Send(humidity);

            //ValueChanged.Send("");

            Sensor.TemperatureSampling = Sampling.UltraHighResolution;
            Sensor.PressureSampling = Sampling.UltraLowPower;
            Sensor.HumiditySampling = Sampling.UltraLowPower;
            Sensor.FilterMode = Bmx280FilteringMode.X2;
        }

        public new void Dispose()
        {
            Stop();

            base.Dispose();
        }
    }
}
