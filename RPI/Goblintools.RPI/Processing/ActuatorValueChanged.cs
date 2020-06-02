using System;

namespace Goblintools.RPI.Processing
{
    public class ActuatorValueChanged
    {
        public DateTime Timestamp { get; set; }

        public string Key { get; set; }
        public object Value { get; set; }
        public string Text { get; set; }

        public ActuatorValueChanged(string key, object value, string text = null)
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
