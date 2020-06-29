using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Goblintools.RPI.Authentication
{
    public class AdafruitKey
    {
        public string Username { get; set; }
        public string Key { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static AdafruitKey Create(string json)
        {
            return JsonConvert.DeserializeObject<AdafruitKey>(json);
        }
    }
}
