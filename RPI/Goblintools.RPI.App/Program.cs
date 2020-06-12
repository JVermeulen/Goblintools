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

            var pin = Convert.ToByte(Configuration["Led:Pin"]);

            using (var processor = new RpiProcessor(pin))
            {
                processor.Start();

                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
                {
                    processor.Dispose();
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
