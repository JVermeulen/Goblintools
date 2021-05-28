using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Goblintools
{
    public class SavedVariableFile
    {
        public FileInfo File { get; set; }

        public string Account { get; set; }
        public string Realm { get; set; }
        public string Character { get; set; }

        public DateTime Timestamp { get; set; }
        public long TimestampEpoch => (int)(Timestamp - new DateTime(1970, 1, 1)).TotalSeconds;

        public static List<SavedVariableFile> ReadSavedVariableFiles(string wowPath)
        {
            var result = new List<SavedVariableFile>();

            if (!Directory.Exists(wowPath))
                throw new DirectoryNotFoundException($"Unable to read SavedVariables files. Directory '{wowPath}' does not exist.");

            var wowDir = new DirectoryInfo(wowPath);
            var accountDirs = wowDir.GetDirectories().Where(d => d.Name.StartsWith("_") && d.Name.EndsWith("_")).
                                                      Select(d => new DirectoryInfo(Path.Combine(d.FullName, "WTF", "Account"))).
                                                      SelectMany(d => d.GetDirectories().Where(s => s.Name != "SavedVariables")).
                                                      ToArray();

            foreach (var accountDir in accountDirs)
            {
                var realmDirs = accountDir.GetDirectories().Where(d => d.Name != "SavedVariables");

                foreach (var realmDir in realmDirs)
                {
                    var characterDirs = realmDir.GetDirectories();

                    foreach (var characterDir in characterDirs)
                    {
                        var savedVariablesDir = characterDir.GetDirectories("SavedVariables").FirstOrDefault();

                        var file = savedVariablesDir?.GetFiles("Goblintools.GPS.lua").FirstOrDefault();

                        if (file != null)
                        {
                            var item = new SavedVariableFile
                            {
                                File = file,
                                Timestamp = System.IO.File.GetLastWriteTime(file.FullName),
                                Account = accountDir.Name,
                                Realm = realmDir.Name,
                                Character = characterDir.Name,
                            };

                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        public void CopyToDestination(string destinationPath)
        {
            CopyToDestination(this, destinationPath);
        }

        public static void CopyToDestination(SavedVariableFile file, string destinationPath)
        {
            //Create destination directory if needed
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            var archiveFileName = Path.Combine(destinationPath, $"{file.Realm} - {file.Character}.zip");
            var entryName = $"{file.Realm}.{file.Character}.{file.TimestampEpoch}.lua";

            CopyToDestination(archiveFileName, file.File.FullName, entryName);

            //using (var zipStream = new FileStream(zipFileName, FileMode.OpenOrCreate))
            //using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Update))
            //{
            //    using (var input = file.File.OpenRead())
            //    {
            //        var entryName = $"{file.Realm}.{file.Character}.{file.TimestampEpoch}.lua";

            //        var entry = archive.GetEntry(entryName);

            //        if (entry == null)
            //        {
            //            entry = archive.CreateEntry(entryName);

            //            using (var output = entry.Open())
            //            {
            //                input.CopyTo(output);
            //            }
            //        }
            //    }
            //}

            var fileName = Path.Combine(destinationPath, entryName);
            file.File.CopyTo(fileName, true);
        }

        public static void CopyToDestination(string archiveFileName, string sourceFileName, string entryName)
        {
            using (var zipStream = new FileStream(archiveFileName, FileMode.OpenOrCreate))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Update))
            {
                var file = new FileInfo(sourceFileName);

                using (var input = file.OpenRead())
                {
                    var entry = archive.GetEntry(entryName);

                    if (entry == null)
                    {
                        entry = archive.CreateEntry(entryName);

                        using (var output = entry.Open())
                        {
                            input.CopyTo(output);
                        }
                    }
                }
            }
        }
    }
}
