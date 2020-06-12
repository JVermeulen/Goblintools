using System;
using System.Reactive.Concurrency;

namespace Goblintools.RPI.Processing
{
    public abstract class Processor : IDisposable
    {
        public static string TimespanFormat { get; set; } = @"dd\.hh\:mm\:ss";

        private HeartbeatGenerator Generator { get; set; }
        protected EventLoopScheduler Scheduler { get; set; }

        public virtual string FriendlyName { get; set; }
        public virtual string Code => this.GetType().Name;

        public Inbox<object> Inbox { get; private set; }

        public bool IsRunning => StartedAt.HasValue && !StoppedAt.HasValue;
        public DateTime? StartedAt { get; private set; }
        public DateTime? StoppedAt { get; private set; }

        public Processor(string friendlyName, TimeSpan heartbeatInterval = default)
        {
            FriendlyName = friendlyName;
            Scheduler = new EventLoopScheduler();

            Inbox = new Inbox<object>(Scheduler);
            Inbox.OnReceive.Subscribe(Work);

            if (heartbeatInterval != default)
            {
                Generator = new HeartbeatGenerator(FriendlyName, heartbeatInterval);
                Generator.OnReceive.Subscribe(Inbox.Send);
            }
        }

        public virtual void Start()
        {
            WriteToConsole($"{FriendlyName} ({Code}) started.", ConsoleColor.Yellow);

            StartedAt = DateTime.Now;
            StoppedAt = null;

            Generator?.Start(Scheduler);
        }

        public virtual void Stop()
        {
            if (!StoppedAt.HasValue)
            {
                StoppedAt = DateTime.Now;

                var duration = GetDuration();

                if (duration.HasValue)
                    WriteToConsole($"{FriendlyName} stopped after {duration.Value.ToString(TimespanFormat)}.", ConsoleColor.Yellow);
                else
                    WriteToConsole($"{FriendlyName} stoped.", ConsoleColor.Yellow);

                Generator?.Stop();
            }
        }

        public virtual void Work(object value)
        {
            //
        }

        public TimeSpan? GetDuration()
        {
            if (StartedAt.HasValue && StoppedAt.HasValue)
                return StoppedAt.Value - StartedAt.Value;
            else if (StartedAt.HasValue && !StoppedAt.HasValue)
                return DateTime.Now - StartedAt.Value;
            else
                return default;
        }

        public void Dispose()
        {
            Stop();

            Scheduler?.Dispose();
            Inbox?.Dispose();
        }

        public void WriteToConsole(string value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ResetColor();
        }

    }
}
