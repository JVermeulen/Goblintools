using Goblintools.RPI.Processing;
using Iot.Units;
using System;

namespace Goblintools.RPI
{
    public class RpiProcessor : Processor, IDisposable
    {
        public RpiProcessor()
        {
            //
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Work(object value)
        {
            if (value is string text)
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

            base.Dispose();
        }
    }
}
