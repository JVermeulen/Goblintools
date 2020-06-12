using System;
using System.Collections.Generic;
using System.Text;

namespace Goblintools.RPI.Processing
{
    public class Observation
    {
        public DateTime Timestamp { get; set; }
        public bool IsSensor { get; set; }
        public string MachineName { get; set; }
        public string DeviceName { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string Text { get; set; }

        public Observation(bool isSensor, string name, object value, string text, string deviceName)
        {
            IsSensor = isSensor;
            Timestamp = DateTime.Now;
            MachineName = Environment.MachineName;

            DeviceName = deviceName;
            Name = name;
            Value = value;
            Text = text;
        }

        public override string ToString()
        {
            return $"{MachineName}.{DeviceName}.{Name} = {Text ?? Value}.";
        }
    }
}
