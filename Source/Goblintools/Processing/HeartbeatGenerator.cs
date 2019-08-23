using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace Goblintools.Processing
{
    public class HeartbeatGenerator : Observer<Heartbeat>
    {
        public TimeSpan Interval { get; private set; }

        private long Value { get; set; }
        private CancellationTokenSource Cancellation { get; set; }

        public HeartbeatGenerator(string name, TimeSpan interval) : base(name)
        {
            Interval = interval;
        }

        public void Start(IScheduler scheduler)
        {
            OnNext(Value);

            Cancellation = new CancellationTokenSource();

            Observable
                .Interval(Interval)
                .ObserveOn(scheduler)
                .Subscribe(OnNext, Cancellation.Token);
        }

        public void Stop()
        {
            Cancellation?.Cancel();
        }

        private void OnNext(long count)
        {
            var value = new Heartbeat(Name, Value++);

            Send(value);
        }
    }
}
