using System;
using System.Globalization;
using System.Threading;
using Topshelf;

namespace Goblintools.App
{
    class Program
    {
        private static Thread MainThread { get; set; }

        private static string Realm { get; set; } = "Darkmoon Faire";
        private static string Character { get; set; } = "Draque";

        static void Main(string[] args)
        {
            MainThread = new Thread(StartService);
            MainThread.Start();
        }

        private static void StartService()
        {
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                var rc = HostFactory.Run(x =>
                {
                    x.Service<GTService>(s =>
                    {
                        s.ConstructUsing(name => new GTService("Hydraxian Waterlords", "Veneficus"));
                        s.WhenStarted(ts => ts.Start());
                        s.WhenStopped(ts => ts.Stop());
                    });
                    x.RunAsLocalSystem();

                    x.SetDescription("Goblintools.App");
                    x.SetDisplayName("Goblintools.App");
                    x.SetServiceName("Goblintools.App");
                });

                var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
                Environment.ExitCode = exitCode;
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
            }
        }
    }
}
