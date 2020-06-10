using Goblintools.RPI.Processing;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.FilteringMode;
using Iot.Device.Bmxx80.PowerMode;
using System;
using System.Threading;

namespace Goblintools.RPI.Sensors
{
    public class Bme280Sensor : I2cSensor, IDisposable
    {
        //https://www.adafruit.com/product/2652
        private Bme280 Product { get; set; }

        public Observation Temperature { get; private set; }
        public Observation Pressure { get; private set; }
        public Observation Humidity { get; private set; }

        public Bme280Sensor(string friendlyName) : base(friendlyName, Bme280.DefaultI2cAddress)
        {
            Product = new Bme280(Device);
            Product.SetPowerMode(Bmx280PowerMode.Forced);
        }

        public override void Start()
        {
            base.Start();

            Read();
        }

        public override void Work(object value)
        {
            if (value is Heartbeat heartbeat)
                Read();
        }

        public void Read()
        {
            if (!base.IsRunning)
                throw new ApplicationException("Unable to read. Sensor has not started yet.");

            // set higher sampling
            Product.TemperatureSampling = Sampling.LowPower;
            Product.PressureSampling = Sampling.UltraHighResolution;
            Product.HumiditySampling = Sampling.Standard;

            // set mode forced so device sleeps after read
            Product.SetPowerMode(Bmx280PowerMode.Forced);

            // wait for measurement to be performed
            var measurementTime = Product.GetMeasurementDuration();
            Thread.Sleep(measurementTime);

            if (Product.TryReadTemperature(out var temperature))
            {
                Temperature = new Observation("Temperature", temperature.Celsius, $"{Math.Round(temperature.Celsius, 2)}°C");

                ValueChanged.Send(Temperature);
            }

            if (Product.TryReadPressure(out var pressure))
            {
                Pressure = new Observation("Pressure", pressure.Hectopascal, $"{Math.Round(pressure.Hectopascal, 0)}hPa");

                ValueChanged.Send(Pressure);
            }

            if (Product.TryReadHumidity(out var humidity))
            {
                Humidity = new Observation("Humidity", humidity, $"{Math.Round(humidity, 1)}%");

                ValueChanged.Send(Humidity);
            }

            Product.TemperatureSampling = Sampling.UltraHighResolution;
            Product.PressureSampling = Sampling.UltraLowPower;
            Product.HumiditySampling = Sampling.UltraLowPower;
            Product.FilterMode = Bmx280FilteringMode.X2;
        }
    }
}
