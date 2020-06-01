using System;
using System.Diagnostics;

namespace Goblintools.RPI.Processing
{
    public class Progress : IDisposable
    {
        public string Message { get; private set; }
        public int Current { get; private set; }
        public int Total { get; private set; }
        public double Percentage => Total > 0 ? ((double)Current / (double)Total) : 0;

        public int Left { get; set; }
        public int Top { get; set; }
        public DateTime BeginTimestamp { get; private set; }
        public DateTime EndTimestamp { get; private set; }

        public bool HasConsole => Process.GetCurrentProcess().MainWindowHandle == IntPtr.Zero;

        public Progress(string message, int total)
        {
            Message = message;
            Current = 0;
            Total = total;

            if (HasConsole)
            {
                Console.Write($"{message}: ");

                Left = Console.CursorLeft;
                Top = Console.CursorTop;
            }

            BeginTimestamp = DateTime.Now;
        }

        public void Increment()
        {
            Current++;

            EndTimestamp = DateTime.Now;

            if (HasConsole)
            {
                Console.SetCursorPosition(Left, Top);
                Console.Write(ToString());
                Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft - 1));
                Console.SetCursorPosition(Left, Top);
            }
        }

        public override string ToString()
        {
            var duration = EndTimestamp - BeginTimestamp;
            var fps = Current / duration.TotalSeconds;

            if (Current < Total)
                return $"{Percentage.ToString("P1")} ({fps.ToString("F1")} fps)";
            else
                return $"{Message}: {duration.ToString(@"hh\:mm\:ss")} ({fps.ToString("F2")} fps)";
        }

        public void Dispose()
        {
            if (HasConsole)
            {
                Console.SetCursorPosition(0, Top);
                Console.Write(ToString());
                Console.WriteLine(new string(' ', Console.WindowWidth - Console.CursorLeft - 1));
            }
        }
    }
}
