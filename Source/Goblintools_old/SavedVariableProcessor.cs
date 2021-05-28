using Goblintools.Processing;
using Goblintools.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblintools
{
    public class SavedVariableProcessor : Processor
    {
        private SavedVariableFileWatcher Watcher { get; set; }
        public Observer<GTGoblinDB> OnGoblinDBChanged { get; set; }

        public SavedVariableProcessor()
        {
            Watcher = new SavedVariableFileWatcher(Properties.Settings.Default.WowPath, TimeSpan.FromSeconds(Properties.Settings.Default.PollInterval));
            Watcher.OnFileChanged.Subscribe(Inbox.Send);
            OnGoblinDBChanged = new Observer<GTGoblinDB>();
        }

        public override void Start()
        {
            base.Start();

            Watcher?.Start();
        }

        public override void Stop()
        {
            Watcher?.Stop();

            base.Stop();
        }

        public void ReadLuaFiles(string path, string realm, string character)
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles($"{realm}.{character}.*.lua");

            var items = files.Select(f => new SavedVariableFile
            {
                File = f,
                Timestamp = File.GetLastWriteTime(f.FullName),
                Realm = realm,
                Character = character
            }).ToList();

            Inbox.Send(items);
        }

        public override void Work(object value)
        {
            base.Work(value);

            if (value is SavedVariableFile file)
            {
                Log.Information($"Updated character {file.Character} from file '{file.File.FullName}'.");

                file.CopyToDestination(Properties.Settings.Default.InputPath);

                var goblinDB = SavedVariableFileParser.Parse(file, file.TimestampEpoch);

                Inbox.Send(goblinDB);
            }
            else if (value is GTGoblinDB goblinDB)
            {
                goblinDB.Save(Properties.Settings.Default.InputPath);

                OnGoblinDBChanged.Send(goblinDB);
            }
            else if (value is List<SavedVariableFile> files)
            {
                var list = files.Select(f => SavedVariableFileParser.Parse(f, f.TimestampEpoch)).OrderBy(g => g.Timestamp).ToList();

                GTGoblinDB.CreateVideo(list, Properties.Settings.Default.OutputPath);
            }
        }
    }
}
