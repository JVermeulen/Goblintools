using Goblintools.RPI.Actors;
using Goblintools.RPI.Processing;
using Goblintools.RPI.Sensors;
using System;
using System.Collections.Generic;
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
            if (observation.IsSensor)
                WriteToConsole($"Sensor value: {observation}", ConsoleColor.Blue);
            else
                WriteToConsole($"Actor value: {observation}", ConsoleColor.Green);
        }

        private void OnHeartbeat(Heartbeat heartbeat)
        {
            var value = Math.Round((double)BME280.Temperature.Value);

            SevenSegment.SetValue($"{value}°C");
        }

        public new void Dispose()
        {
            RedLED?.Dispose();
            SevenSegment?.Dispose();
            BME280?.Dispose();
        }
    }
}
