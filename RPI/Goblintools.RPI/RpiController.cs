using Goblintools.RPI.Actors;
using Goblintools.RPI.Logic;
using Goblintools.RPI.Processing;
using Goblintools.RPI.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Goblintools.RPI
{
    public class RpiController : Processor
    {
        public JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

        public LedActor RedLED { get; set; }
        public SevenSegmentActor SevenSegment { get; set; }
        public Bme280Sensor BME280 { get; set; }
        public VCNL4000Sensor VCNL4000 { get; set; }
        public Generator Generator { get; set; }
        public DomoticzApi API { get; set; }

        public RpiController() : base("RPI Controller", TimeSpan.FromSeconds(15))
        {
            RedLED = new LedActor("Red led", 24);
            RedLED.ValueChanged.OnReceive.Subscribe(Work);

            SevenSegment = new SevenSegmentActor("7-Segment display");
            SevenSegment.ValueChanged.OnReceive.Subscribe(Work);

            BME280 = new Bme280Sensor("Temperature, Humidity and Pressure Sensor");
            BME280.ValueChanged.OnReceive.Subscribe(Work);

            VCNL4000 = new VCNL4000Sensor("Light");
            VCNL4000.ValueChanged.OnReceive.Subscribe(Work);

            Generator = new Generator();
            Generator.ValueChanged.OnReceive.Subscribe(Work);

            API = new DomoticzApi();

            Start();
        }

        public override void Start()
        {
            RedLED.Start();
            SevenSegment.Start();
            BME280.Start();
            VCNL4000.Start();

            //Generator.Start();

            base.Start();
        }

        public override void Work(object value)
        {
            if (value is Heartbeat heartbeat)
                OnHeartbeat(heartbeat);
            else if (value is Observation observation)
                OnObservation(observation);
        }

        private void OnObservation(Observation observation)
        {
            if (Generator != null && observation.DeviceName == Generator.Code)
                SevenSegment.SetValue(observation.Value.ToString());
            else if (observation.Category == "Sensor")
            {
                WriteToConsole($"Sensor value: {observation}", ConsoleColor.Blue);

                if (observation.Name == "Temperature")
                    API.UpdateDevice("http://192.168.2.204:8080", 1, observation.Value, observation.Text);
                else if (observation.Name == "Humidity")
                    API.UpdateDevice("http://192.168.2.204:8080", 3, observation.Value, observation.Text);
                else if (observation.Name == "Pressure")
                    API.UpdateDevice("http://192.168.2.204:8080", 4, (double)observation.Value / 1000, observation.Text);
            }
            else if (observation.Category == "Actor")
                WriteToConsole($"Actor value: {observation}", ConsoleColor.Green);
            else
                WriteToConsole($"{observation.Category} value: {observation}", ConsoleColor.White);
        }

        private void OnHeartbeat(Heartbeat heartbeat)
        {
            if (BME280.Temperature != null && BME280.Temperature.Value != null)
            {
                var value = Math.Round((double)BME280.Temperature.Value, 0);

                SevenSegment.SetValue($"{value}°C");
            }
        }

        public List<Observation> SearchObservations(string keyword = null)
        {
            var observations = new List<Observation>()
            {
                 BME280.Temperature,
                 BME280.Pressure,
                 BME280.Humidity,
                 VCNL4000.AmbientLight,
                 VCNL4000.Proximity,
                 RedLED.LED,
                 SevenSegment.SevenSegment,
            };

            if (keyword == null)
                return observations.Where(o => o != null).ToList();
            else
                return observations.Where(o => o != null && o.Contains(keyword)).ToList();
        }

        public List<Observation> GetSensors()
        {
            return new List<Observation>()
            {
                 BME280.Temperature,
                 BME280.Pressure,
                 BME280.Humidity,
                 VCNL4000.AmbientLight,
                 VCNL4000.Proximity,
            };
        }

        public List<Observation> GetActors()
        {
            return new List<Observation>()
            {
                 RedLED.LED,
                 SevenSegment.SevenSegment,
            };
        }

        public List<HardwareDevice> GetHardwareDevices()
        {
            return new List<HardwareDevice>
            {
                BME280.HardwareDevice,
                VCNL4000.HardwareDevice,
                RedLED.HardwareDevice,
                SevenSegment.HardwareDevice,
                new HardwareDevice
                {
                    Name = "I2C",
                    Description="Active I2C addresses",
                    Type = "Communication",
                    Address=string.Join(", ", I2cDetector.GetActiveAddresses().Select(a => $"0x{a.ToString("X2")}")),
                    Reference  = "https://learn.adafruit.com/i2c-addresses",
                }
            };
        }

        public List<int> I2cDetect()
        {
            return I2cDetector.GetActiveAddresses();
        }

        public new void Dispose()
        {
            Generator?.Dispose();
            RedLED?.Dispose();
            SevenSegment?.Dispose();
            BME280?.Dispose();
            VCNL4000?.Dispose();
        }
    }
}
