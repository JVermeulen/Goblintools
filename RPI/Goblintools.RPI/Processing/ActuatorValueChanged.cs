using System;

namespace Goblintools.RPI.Processing
{
    public class ActuatorValueChanged
    {
        public DateTime Timestamp { get; set; }

        public string Key { get; set; }
        public object Value { get; set; }

        public ActuatorValueChanged(string key, object value)
        {
            Timestamp = DateTime.Now;

            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Key} set to '{Value}'.";
        }
    }
}
