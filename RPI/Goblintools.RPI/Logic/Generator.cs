using Goblintools.RPI.Processing;
using System;

namespace Goblintools.RPI.Logic
{
    public class Generator : Processor
    {
        public override string Category => "System";
        public override string Code => "Generator";

        public Observer<Observation> ValueChanged { get; set; }

        public Generator(): base("Generator", TimeSpan.FromSeconds(1))
        {
            ValueChanged = new Observer<Observation>();
        }

        public override void Work(object value)
        {
            if (value is Heartbeat heartbeat)
                OnHeartbeat(heartbeat);
        }

        private void OnHeartbeat(Heartbeat heartbeat)
        {
            var value = DateTime.Now.ToString("HH:mm").PadLeft(5);

            var observation = new Observation(Category, FriendlyName, value, value, Code);

            ValueChanged.Send(observation);
        }
    }
}
