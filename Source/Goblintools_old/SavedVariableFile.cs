using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            //var classicAccountDir = new DirectoryInfo(Path.Combine(wowPath, "_classic_", "WTF", "Account"));
            //var retailAccountDir = new DirectoryInfo(Path.Combine(wowPath, "_retail_", "WTF", "Account"));

            //var classicDirs = classicAccountDir.GetDirectories().Where(d => d.Name != "SavedVariables");
            //var retailDirs = retailAccountDir.GetDirectories().Where(d => d.Name != "SavedVariables");
            //var accountDirs = retailDirs.Union(classicDirs).ToArray();

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
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            var fileName = Path.Combine(destinationPath, $"{file.Realm}.{file.Character}.{file.TimestampEpoch}.lua");

            file.File.CopyTo(fileName, true);
        }
    }
}
