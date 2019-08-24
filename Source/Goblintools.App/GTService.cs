using Goblintools.Processing;
using Goblintools.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Goblintools.App
{
    public class GTService : Processor
    {
        private SavedVariableFileWatcher Watcher { get; set; }

        public GTService()
        {
            Watcher = new SavedVariableFileWatcher(Properties.Settings.Default.WowPath, TimeSpan.FromSeconds(Properties.Settings.Default.PollInterval));
            Watcher.OnFileChanged.Subscribe(Inbox.Send);
        }

        public GTService(string realm, string character)
        {
            ReadLuaFiles(Properties.Settings.Default.InputPath, realm, character);
        }

        public override void Start()
        {
            Watcher?.Start();

            Console.WriteLine();
        }

        public override void Stop()
        {
            Watcher?.Stop();
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
                file.CopyToDestination(Properties.Settings.Default.InputPath);

                var goblinDB = SavedVariableFileParser.Parse(file, file.TimestampEpoch);

                Inbox.Send(goblinDB);
            }
            else if (value is GTGoblinDB goblinDB)
            {
                goblinDB.Save(Properties.Settings.Default.InputPath);

                //goblinDB.CreateImages(Properties.Settings.Default.OutputPath);
                goblinDB.CreateVideo(Properties.Settings.Default.OutputPath);
            }
            else if (value is List<SavedVariableFile> files)
            {
                var list = files.Select(f => SavedVariableFileParser.Parse(f, f.TimestampEpoch)).OrderBy(g => g.Timestamp).ToList();

                GTGoblinDB.CreateVideo(list, Properties.Settings.Default.OutputPath);
            }
        }
    }
}
