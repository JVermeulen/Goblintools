using Goblintools.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Goblintools
{
    public class SavedVariableFileWatcher : Processor
    {
        public string WowPath { get; private set; }
        private List<SavedVariableFile> Current { get; set; }

        private Subject<SavedVariableFile> FileChanged { get; set; }
        public IObservable<object> OnFileChanged => FileChanged.ObserveOn(Scheduler).AsObservable();

        public SavedVariableFileWatcher(string wowPath, TimeSpan interval) : base(interval)
        {
            WowPath = wowPath;

            Current = new List<SavedVariableFile>();
            FileChanged = new Subject<SavedVariableFile>();

            Log.Information($"Watching directory '{wowPath}' every {(int)interval.TotalSeconds} seconds.");
        }

        public override void Work(object value)
        {
            try
            {
                var files = SavedVariableFile.ReadSavedVariableFiles(WowPath);

                foreach (var file in files)
                {
                    var current = Current.Where(f => f.File.FullName == file.File.FullName).FirstOrDefault();

                    if (current == null)
                    {
                        Current.Add(file);

                        FileChanged.OnNext(file);
                    }
                    else if (current.Timestamp != file.Timestamp)
                    {
                        current.Timestamp = file.Timestamp;

                        FileChanged.OnNext(file);
                    }
                    else
                    {
                        //
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
