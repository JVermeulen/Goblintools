using System;
using System.Collections.Generic;
using System.Text;

namespace Goblintools.RPI.Processing
{
    public class Observation
    {
        public DateTime Timestamp { get; set; }

        public string Key { get; set; }
        public object Value { get; set; }
        public string Text { get; set; }

        public Observation(string key, object value, string text = null)
        {
            Timestamp = DateTime.Now;

            Key = key;
            Value = value;
            Text = text;
        }

        public override string ToString()
        {
            return $"{Key} set to '{Text ?? Value}'.";
        }
    }
}
