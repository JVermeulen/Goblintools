using Goblintools.RPI.Actuators;
using Goblintools.RPI.Processing;
using Iot.Units;
using System;

namespace Goblintools.RPI
{
    public class RpiProcessor : Processor, IDisposable
    {
        public SingleLed LED { get; private set; }

        public RpiProcessor(int ledPin)
        {
            LED = new SingleLed(ledPin)
            {
                BlinkLed = true,
            };

            LED.ValueChanged.OnReceive.Subscribe(Work);
        }

        public override void Start()
        {
            Console.WriteLine($"{Name} started.");
            
            LED.Start();

            base.Start();

            Console.WriteLine();
        }

        public override void Stop()
        {
            Console.WriteLine($"{Name} stopped.");

            LED.Stop();

            base.Stop();
        }

        public override void Work(object value)
        {
            if (value is ActuatorValueChanged actuatorValueChanged)
                Console.WriteLine(actuatorValueChanged);
            else if (value is string text)
                Console.WriteLine(text);
            else if (value is Temperature temperature)
                Console.WriteLine($"Temperature: {temperature.Celsius:0.#}\u00B0C");
            else if (value is Pressure pressure)
                Console.WriteLine($"Pressure: {pressure.Hectopascal:0.##}hPa");
            else if (value is double humidity)
                Console.WriteLine($"Relative humidity: {humidity:0.#}%");

            base.Work(value);
        }

        public new void Dispose()
        {
            Stop();

            LED?.Dispose();
            base.Dispose();
        }
    }
}
