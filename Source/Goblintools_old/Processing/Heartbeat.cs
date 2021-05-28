using System;

namespace Goblintools.Processing
{
    public class Heartbeat
    {
        public string Name { get; private set; }
        public long Value { get; private set; }

        public Heartbeat(string name, long value)
        {
            Name = name;
            Value = value;
        }
    }
}
