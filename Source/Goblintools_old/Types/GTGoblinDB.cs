using Goblintools.Processing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Goblintools.Types
{
    public class GTGoblinDB
    {
        public long Timestamp { get; private set; }
        public GTGame Game { get; set; }
        public GTCharacter Character { get; set; }
        public GTLog Records { get; set; }

        public GTGoblinDB(string account,  long timestamp)
        {
            Timestamp = timestamp;

            Game = new GTGame();
            Character = new GTCharacter { Account = account };
            Records = new GTLog();
        }

        public void Save(string destinationPath)
        {
            Save(this, destinationPath);
        }

        public static void Save(GTGoblinDB goblinDB, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            var json = JsonConvert.SerializeObject(goblinDB);

            var archiveFileName = Path.Combine(destinationPath, $"{goblinDB.Character.Realm} - {goblinDB.Character.Name}.zip");
            var entryName = $"{goblinDB.Character.Realm}.{goblinDB.Character.Name}.{goblinDB.Timestamp}.json";

            var fileName = Path.Combine(destinationPath, entryName);

            File.WriteAllText(fileName, json);

            SavedVariableFile.CopyToDestination(archiveFileName, fileName, entryName);
        }

        public void CreateImages(string path)
        {
            CreateImages(this, path);
        }

        public static void CreateImages(GTGoblinDB goblinDB, string path, int max = int.MaxValue)
        {
            using (var creator = new ImageCreator())
            {
                int i = 0;

                foreach (var data in goblinDB.Records.Data)
                {
                    i++;

                    if (i < max)
                    {
                        var bitmap = creator.CreateImage(goblinDB.Records.Format, data);

                        bitmap.Save(Path.Combine(path, $"image{i.ToString("d5")}.png"), ImageFormat.Png);
                    }
                }
            }
        }

        public void CreateVideo(string path = null, int maxFrames = int.MaxValue)
        {
            if (path == null)
                path = Properties.Settings.Default.OutputPath;

            CreateVideo(this, path, maxFrames);
        }

        public static void CreateVideo(GTGoblinDB goblinDB, string path, int maxFrames = int.MaxValue)
        {
            try
            {
                var name = $"{goblinDB.Character.Realm}.{goblinDB.Character.Name}.{goblinDB.Timestamp}.mp4";
                var encoderFileName = Path.Combine(Properties.Settings.Default.DataPath, "Tools", "ffmpeg.exe");
                var videoFileName = Path.Combine(path, name);
                var mapsDirectoryName = Path.Combine(Properties.Settings.Default.DataPath, "Maps");

                if (!File.Exists(encoderFileName))
                    throw new FileNotFoundException($"Unable to create video. Encoder '{encoderFileName}' does not exist.");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (var imageCreator = new ImageCreator())
                using (var videoCreator = new VideoCreator(encoderFileName, videoFileName))
                using (var progress = new Progress(name, goblinDB.Records.Count))
                {
                    foreach (var data in goblinDB.Records.Data)
                    {
                        progress.Increment();

                        if (progress.Current < maxFrames)
                        {
                            var bitmap = imageCreator.CreateImage(goblinDB.Records.Format, data);

                            videoCreator.WriteFrame(bitmap);
                        }
                    }

                    Log.Information($"Finished creating video. {progress.ToString()}");

                    if (imageCreator.MissingMaps.Count > 0)
                    {
                        var maps = string.Join(", ", imageCreator.MissingMaps.ToArray());

                        Log.Warning($"Missings maps in directory '{mapsDirectoryName}': {maps}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static void CreateVideo(List<GTGoblinDB> goblinDBList, string path, int max = int.MaxValue)
        {
            var name = goblinDBList.Select(g => $"{g.Character.Realm}.{g.Character.Name}.mp4").FirstOrDefault();

            var encoderFileName = Path.Combine(Properties.Settings.Default.DataPath, "Tools", "ffmpeg.exe");
            var videoFileName = Path.Combine(path, name);

            using (var imageCreator = new ImageCreator())
            using (var videoCreator = new VideoCreator(encoderFileName, videoFileName))
            using (var progress = new Progress(name, goblinDBList.Sum(g => g.Records.Count)))
            {
                foreach (var goblinDB in goblinDBList)
                {
                    Console.WriteLine($"Converting {goblinDB.Character.Realm}.{goblinDB.Character.Name} ({goblinDB.Timestamp}): ");

                    foreach (var data in goblinDB.Records.Data)
                    {
                        progress.Increment();

                        if (progress.Current < max)
                        {
                            var bitmap = imageCreator.CreateImage(goblinDB.Records.Format, data);

                            videoCreator.WriteFrame(bitmap);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{Character.Realm}.{Character.Name}.{Timestamp}";
        }
    }
}
