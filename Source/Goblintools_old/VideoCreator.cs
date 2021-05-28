using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Goblintools
{
    public class VideoCreator : IDisposable
    {
        public Process FFmpegProcess { get; private set; }
        public ProcessStartInfo FFmpegArgs { get; private set; }
        public int FrameRate { get; set; } = 24;

        private BinaryWriter Writer { get; set; }

        public VideoCreator(string ffmpegFileName, string videoFileName)
        {
            try
            {
                var workingPath = Path.Combine(Properties.Settings.Default.DataPath, "Temp");
                if (!Directory.Exists(workingPath))
                    Directory.CreateDirectory(workingPath);

                var videoFile = new FileInfo(videoFileName);
                if (!videoFile.Directory.Exists)
                    videoFile.Directory.Create();

                FFmpegProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = ffmpegFileName,
                        Arguments = $"-y -framerate {FrameRate} -f image2pipe -i - -r {FrameRate} -c:v libx264 -vf \"fps = {FrameRate}, format = yuv420p\" \"{videoFileName}\"",
                        UseShellExecute=false,
                        CreateNoWindow=true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false
                    },
                    EnableRaisingEvents = true,
                };

                FFmpegProcess.Start();

                Writer = new BinaryWriter(FFmpegProcess.StandardInput.BaseStream);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void WriteFrame(Bitmap bitmap)
        {
            using (var buffer = new MemoryStream())
            {
                bitmap.Save(buffer, ImageFormat.Png);

                Writer?.Write(buffer.ToArray());
            }
        }

        public void Dispose()
        {
            Writer?.Dispose();
            Writer = null;

            FFmpegProcess?.Dispose();
            FFmpegProcess = null;
        }
    }
}
