using Goblintools.RPI.Processing;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.FilteringMode;
using Iot.Device.Bmxx80.PowerMode;
using System;
using System.Threading.Tasks;

namespace Goblintools.RPI.Sensors
{
    public class Bme280Sensor : I2cSensor
    {
        //https://www.adafruit.com/product/2652
        private Bme280 Product { get; set; }

        public Observation Temperature { get; private set; }
        public Observation Pressure { get; private set; }
        public Observation Humidity { get; private set; }
        public Observation Altitude { get; private set; }

        public override string Code => "BME280";

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
            Product.TemperatureSampling = Sampling.UltraHighResolution;
            Product.PressureSampling = Sampling.UltraHighResolution;
            Product.HumiditySampling = Sampling.UltraHighResolution;
            Product.FilterMode = Bmx280FilteringMode.X16;

            // set mode forced so device sleeps after read
            Product.SetPowerMode(Bmx280PowerMode.Forced);

            // wait for measurement to be performed
            var measurementTime = Product.GetMeasurementDuration();
            Task.Delay(measurementTime).Wait();

            if (Product.TryReadTemperature(out var temperature))
            {
                Temperature = new Observation(true, "Temperature", Math.Round(temperature.Celsius, 5), $"{Math.Round(temperature.Celsius, 1)}°C", Code);

                ValueChanged.Send(Temperature);
            }

            if (Product.TryReadPressure(out var pressure))
            {
                Pressure = new Observation(true, "Pressure", Math.Round(pressure.Hectopascal, 5), $"{Math.Round(pressure.Hectopascal, 0)}hPa", Code);

                ValueChanged.Send(Pressure);
            }

            if (Product.TryReadHumidity(out var humidity))
            {
                Humidity = new Observation(true, "Humidity", Math.Round(humidity, 5), $"{Math.Round(humidity, 1)}%", Code);

                ValueChanged.Send(Humidity);
            }

            if (Product.TryReadAltitude(Iot.Units.Pressure.FromHectopascal(1013.25), out double altitude))
            {
                Altitude = new Observation(true, "Altitude", Math.Round(altitude, 5), $"{Math.Round(altitude, 0)}m", Code);

                ValueChanged.Send(Altitude);
            }

            Product.TemperatureSampling = Sampling.UltraLowPower;
            Product.PressureSampling = Sampling.UltraLowPower;
            Product.HumiditySampling = Sampling.UltraLowPower;
            Product.FilterMode = Bmx280FilteringMode.Off;
        }
    }
}
