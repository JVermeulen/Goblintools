using Flurl;
using Goblintools.RPI.Authentication;
using Goblintools.RPI.Processing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Goblintools.RPI
{
    public class AdafruitAPI
    {
        private AdafruitKey Authentication { get; set; }

        public AdafruitAPI()
        {
            Authentication = LoadKeyFromFile();
        }

        public AdafruitKey LoadKeyFromFile()
        {
            var keyFileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "config", "Adafruit.key");
            var keyFile = new FileInfo(keyFileName);

            string json;

            Console.WriteLine(keyFileName);

            if (!keyFile.Directory.Exists)
            {
                Processor.WriteToConsole($"Directory '{keyFile.Directory}' does not exist. Directory but will be created.", ConsoleColor.DarkRed);

                keyFile.Directory.Create();
            }

            if (!File.Exists(keyFileName))
            {
                Processor.WriteToConsole($"File '{keyFileName}' does not exist. A template file but will be created.", ConsoleColor.DarkRed);

                using (var output = File.Create(keyFileName))
                using (var writer = new StreamWriter(output))
                {
                    writer.Write(new AdafruitKey().ToString());
                }
            }

            json = File.ReadAllText(keyFileName);

            return AdafruitKey.Create(json);
        }

        public void SendToServer(string feed, double value)
        {
            if (Authentication?.Username != null)
            {
                var url = $"https://io.adafruit.com/api/v2/{Authentication.Username}/feeds/{feed.ToLower()}/data";
                var body = new { value };
                var headers = new Dictionary<string, string>
                {
                    { "X-AIO-Key", Authentication.Key }
                };

                HttpRequest.TryPost(url, JsonConvert.SerializeObject(body), headers, out string result);
            }
        }
    }
}
