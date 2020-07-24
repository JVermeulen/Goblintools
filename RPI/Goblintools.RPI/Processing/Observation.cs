using System;

namespace Goblintools.RPI.Processing
{
    public class Observation
    {
        public DateTime Timestamp { get; set; }
        public string MachineName { get; set; }
        public string DeviceName { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string Text { get; set; }

        public Observation(string category, string name, object value, string text, string deviceName)
        {
            Category = category;
            Timestamp = DateTime.Now;
            MachineName = Environment.MachineName;

            DeviceName = deviceName;
            Name = name;
            Value = value;
            Text = text;
        }

        public bool Contains(string keyword)
        {
            return keyword == null || 
                keyword.Equals(Category, StringComparison.OrdinalIgnoreCase) || 
                keyword.Equals(MachineName, StringComparison.OrdinalIgnoreCase) || 
                keyword.Equals(DeviceName, StringComparison.OrdinalIgnoreCase) || 
                keyword.Equals(Name, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            if (Text != null || Value != null)
                return $"{MachineName}.{DeviceName}.{Name} = {Text ?? Value}";
            else
                return $"{MachineName}.{DeviceName}.{Name} = NULL";
        }
    }
}
