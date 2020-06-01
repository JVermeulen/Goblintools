using Goblintools.RPI.Actuators;
using Goblintools.RPI.Sensors;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Goblintools.RPI.App
{
    class Program
    {
        public static IConfigurationRoot Configuration;

        static void Main(string[] args)
        {
            LoadConfiguration();

            var pin = Convert.ToInt32(Configuration["Led:Pin"]);

            using (var processor = new RpiProcessor())
            using (var led = new SingleLed(pin))
            using (var display = new HT16K33Display())
            using (var sensor = new Bme280Sensor())
            {
                processor.Start();

                led.ValueChanged.OnReceive.Subscribe(processor.Work);
                led.Start();

                display.ShowTime = true;
                display.Start();

                sensor.ValueChanged.OnReceive.Subscribe(processor.Work);
                sensor.ValueChanged.OnReceive.Subscribe(led.Work);
                sensor.Start();

                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
                {
                    processor.Dispose();
                    led.Dispose();
                    display.Dispose();
                    sensor.Dispose();
                };

                Task.Delay(-1).Wait();
            }
        }

        static void LoadConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();
        }
    }
}
