using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Goblintools.RPI.Processing
{
    public class Observer<T> : IDisposable
    {
        public string Name { get; set; }

        private Subject<T> Value { get; set; }
        public IObservable<T> OnReceive => Value.AsObservable();

        public Observer(string name = null)
        {
            Name = name ?? typeof(T).Name;
            Value = new Subject<T>();
        }

        public void Send(T value)
        {
            Value.OnNext(value);
        }

        public void Dispose()
        {
            Value?.Dispose();
        }
    }
}
