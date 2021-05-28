using Goblintools.Types;
using System;
using System.IO;
using System.Linq;

namespace Goblintools
{
    public class SavedVariableFileParser
    {
        public static GTGoblinDB Parse(SavedVariableFile file, long timestamp)
        {
            GTGoblinDB goblinDB = new GTGoblinDB(file.Account, timestamp);

            using (var input = new FileStream(file.File.FullName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(input))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (TryRead(line, out string key, out string value))
                    {
                        if (key == "expansion")
                            goblinDB.Game.Expansion = int.Parse(value);
                        else if (key == "api")
                            goblinDB.Game.API = int.Parse(value);
                        else if (key == "name")
                            goblinDB.Character.Name = value;
                        else if (key == "realm")
                            goblinDB.Character.Realm = value;
                        else if (key == "race")
                            goblinDB.Character.Race = value;
                        else if (key == "class")
                            goblinDB.Character.Class = value;
                        else if (key == "played")
                            goblinDB.Character.Played = long.Parse(value);
                        else if (key == "format")
                            goblinDB.Records.Format = value;
                        else if (key == "beginTime")
                            goblinDB.Records.BeginTime = ParseEpoch(long.Parse(value));
                        else if (key == "endTime")
                            goblinDB.Records.EndTime = ParseEpoch(long.Parse(value));
                        else if (key == "count")
                            goblinDB.Records.Count = int.Parse(value);
                        else if (key == "record")
                            goblinDB.Records.Data.Add(value);
                    }
                }
            }

            return goblinDB;
        }

        public static DateTime ParseEpoch(long epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(epoch);
        }

        public static bool TryRead(string line, out string key, out string value)
        {
            key = null;
            value = null;

            try
            {
                if (string.IsNullOrWhiteSpace(line))
                    return false;

                line = line.Trim();

                if (line.Contains('='))
                {
                    int index = line.IndexOf('=');

                    string left = line.Substring(0, index - 1)?.Trim();
                    string right = line.Substring(index + 1, line.Length - index - 2)?.Trim();

                    if (left.StartsWith("[\"") && left.EndsWith("\"]"))
                        key = left.Substring(2, left.Length - 4);

                    if (right.StartsWith("\"") && right.EndsWith("\""))
                        value = right.Substring(1, right.Length - 2);
                    else
                        value = right;
                }
                else if (line.StartsWith("\""))
                {
                    int i0 = line.IndexOf('\"');
                    int i1 = line.LastIndexOf("\"");

                    key = "record";
                    value = line.Substring(i0 + 1, i1 - i0 - 1);
                }

                return (key != null && value != null);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
