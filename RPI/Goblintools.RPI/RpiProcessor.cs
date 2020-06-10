using Goblintools.RPI.Actuators;
using Goblintools.RPI.Processing;
using Goblintools.RPI.Sensors;
using Iot.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Goblintools.RPI
{
    public class RpiProcessor : Processor, IDisposable
    {
        public HT16K33Display Display { get; private set; }

        private int RotateIndex { get; set; }
        public Dictionary<string, string> RotateText { get; set; }

        public RpiProcessor(byte pin = 24, int interval = 5) : base("Main Controller", TimeSpan.FromSeconds(interval))
        {
            RotateText = new Dictionary<string, string>();

            Display = new HT16K33Display("7-Segment");
            Display.ValueChanged.OnReceive.Subscribe(Work);
        }

        public override void Start()
        {
            base.Start();

            Display.Start();

            Console.WriteLine();
        }

        public override void Stop()
        {
            Console.WriteLine();

            Display.Stop();

            base.Stop();
        }

        public override void Work(object value)
        {
            if (value is Heartbeat heartbeat)
                OnHeartbeat(heartbeat);
            else if (value is Observation valueChangedMessage)
                WriteToConsole($"{valueChangedMessage}", ConsoleColor.Blue);
            else if (value is string text)
                WriteToConsole($"{text}", ConsoleColor.White);
            else if (value is Temperature temperature)
                OnTemperatureReceived(temperature);
            else if (value is Pressure pressure)
                OnPressureReceived(pressure);
            else if (value is double humidity)
                OnHumidityReceived(humidity);

            base.Work(value);
        }

        public void OnHeartbeat(Heartbeat heartbeat)
        {
            SetRotateText("Time", DateTime.Now.ToString("HH:mm").PadLeft(5));

            RotateIndex++;

            if (RotateIndex >= RotateText.Count)
                RotateIndex = 0;

            Display.Inbox.Send(RotateText.Values.ElementAt(RotateIndex));
        }

        public void OnTemperatureReceived(Temperature value)
        {
            WriteToConsole($"Temperature: {value.Celsius:0.#}\u00B0C", ConsoleColor.Green);

            SetRotateText("Temperature", $"{Math.Round(value.Celsius, 0)}°C");
        }

        private void OnPressureReceived(Pressure value)
        {
            WriteToConsole($"Pressure: {value.Hectopascal:0.##}hPa", ConsoleColor.Green);

            SetRotateText("Pressure", $"{Math.Round(value.Hectopascal, 0)}");
        }

        private void OnHumidityReceived(double value)
        {
            WriteToConsole($"Relative humidity: {value:0.#}%", ConsoleColor.Green);

            SetRotateText("Humidity", $"{Math.Round(value, 0)}°o");
        }

        private void SetRotateText(string key, string value)
        {
            if (RotateText.ContainsKey(key))
                RotateText[key] = value;
            else
                RotateText.Add(key, value);
        }

        public new void Dispose()
        {
            Stop();

            Display?.Dispose();

            base.Dispose();
        }
    }
}
