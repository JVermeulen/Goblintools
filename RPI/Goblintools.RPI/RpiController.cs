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
        public Generator Generator { get; set; }

        public RpiController() : base("RPI Controller", TimeSpan.FromSeconds(15))
        {
            RedLED = new LedActor("Red led", 24);
            RedLED.ValueChanged.OnReceive.Subscribe(Work);
            RedLED.Start();

            SevenSegment = new SevenSegmentActor("7-Segment display");
            SevenSegment.ValueChanged.OnReceive.Subscribe(Work);
            SevenSegment.Start();

            BME280 = new Bme280Sensor("Temperature, Humidity and Pressure Sensor");
            BME280.ValueChanged.OnReceive.Subscribe(Work);
            BME280.Start();

            Generator = new Generator();
            Generator.ValueChanged.OnReceive.Subscribe(Work);
            //Generator.Start();

            Start();
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
                WriteToConsole($"Sensor value: {observation}", ConsoleColor.Blue);
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

        public List<Observation> GetObservations()
        {
            var observations = new List<Observation>()
            {
                 BME280.Temperature,
                 BME280.Pressure,
                 BME280.Humidity,
                 RedLED.LED,
                 SevenSegment.SevenSegment,
            };

            return observations.Where(o => o != null).ToList();
        }

        public new void Dispose()
        {
            Generator?.Dispose();
            RedLED?.Dispose();
            SevenSegment?.Dispose();
            BME280?.Dispose();
        }
    }
}
