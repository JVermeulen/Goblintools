using Goblintools.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Goblintools
{
    public class ImageCreator : IDisposable
    {
        public string DataPath { get; set; }

        public int DefaultWidth { get; set; }
        public int DefaultHeight { get; set; }

        public int LastMapID { get; private set; }
        public Bitmap BaseMap { get; private set; }

        public List<int> MissingMaps { get; private set; }

        public ImageCreator(int defaultWidth = 1002, int defaultHeight = 668)
        {
            DataPath = Properties.Settings.Default.DataPath;

            DefaultWidth = defaultWidth;
            DefaultHeight = defaultHeight;

            MissingMaps = new List<int>();
        }

        public Bitmap CreateImage(string format, string data)
        {
            var record = GTRecord.Create(format, data);

            if (!SetBaseMap(record.Map) && !MissingMaps.Contains(record.Map))
                MissingMaps.Add(record.Map);

            var bitmap = (Bitmap)BaseMap.Clone();
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var x = (int)(record.X * bitmap.Width / 100);
                var y = (int)(record.Y * bitmap.Height / 100);

                var state = record.GetState();

                var player = GetPlayerImage(state);
                graphics.DrawImageUnscaled(player, x - player.Width / 2, y - player.Height / 2);

                graphics.DrawString(record.Level.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 20, 20);

                graphics.Save();
            }

            return bitmap;
        }

        public bool SetBaseMap(int mapID)
        {
            if (mapID != LastMapID || BaseMap == null)
            {
                LastMapID = mapID;

                string defaultFileName = Path.Combine(DataPath, "Maps", $"0.png");
                string classicMapFileName = Path.Combine(DataPath, "Maps", "_classic_", $"{LastMapID}.png");
                string retailMapFileName = Path.Combine(DataPath, "Maps", "_retail_", $"{LastMapID}.jpg");

                if (File.Exists(classicMapFileName))
                {
                    BaseMap = new Bitmap(classicMapFileName);

                    return true;
                }
                else if (File.Exists(retailMapFileName))
                {
                    BaseMap = new Bitmap(retailMapFileName);

                    return true;
                }
                else if (File.Exists(defaultFileName))
                {
                    BaseMap = new Bitmap(defaultFileName);

                    return false;
                }
                else
                {
                    BaseMap = new Bitmap(DefaultWidth, DefaultHeight);

                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public Image GetPlayerImage(GTPlayerState state)
        {
            switch (state)
            {
                case GTPlayerState.Dead:
                    return Image.FromFile(Path.Combine(DataPath, "Style", "player_death.png"));
                case GTPlayerState.Combat:
                    return Image.FromFile(Path.Combine(DataPath, "Style", "player_combat.png"));
                case GTPlayerState.Taxi:
                    return Image.FromFile(Path.Combine(DataPath, "Style", "player_taxi.png"));
                case GTPlayerState.Mounted:
                    return Image.FromFile(Path.Combine(DataPath, "Style", "player_mounted.png"));
                case GTPlayerState.Resting:
                    return Image.FromFile(Path.Combine(DataPath, "Style", "player_resting.png"));
                default:
                    return Image.FromFile(Path.Combine(DataPath, "Style", "player.png"));
            }
        }

        public void Dispose()
        {
            BaseMap?.Dispose();
        }
    }
}
