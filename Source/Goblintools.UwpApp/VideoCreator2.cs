using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace Goblintools.UwpApp
{
    public class VideoCreator2
    {
        public MediaStreamSource Source { get; set; }

        public VideoCreator2()
        {
            var properties = VideoEncodingProperties.CreateH264(); //.CreateUncompressed(CodecSubtypes.VideoFormatH265, 1002, 668);
            var descriptor = new VideoStreamDescriptor(properties);

            Source = new MediaStreamSource(descriptor);
            Source.SampleRequested += Source_SampleRequested;
        }

        private void Source_SampleRequested(MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            var deferal = args.Request.GetDeferral();

            try
            {
                var file = StorageFile.GetFileFromPathAsync(@"C:\Users\JoostVermeulenTensin\Pictures\Goblintools\947.png");
                //var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\grpPC1.jpg");

                //BitmapImage image = new BitmapImage(new Uri(@"C:\Users\JoostVermeulenTensin\Pictures\Goblintools\947.png"));
                var data = LoadFile(@"C:\Users\JoostVermeulenTensin\Pictures\Goblintools\947.png").Result;

                args.Request.Sample = MediaStreamSample.CreateFromBuffer(data, new TimeSpan(0));
                args.Request.Sample.Duration = TimeSpan.FromSeconds(3);
            }
            finally
            {
                deferal.Complete();
            }

            //args.Request.Sample = MediaStreamSample.CreateFromBuffer(buffer, );
        }

        private async Task<Windows.Storage.Streams.IBuffer> LoadFile(string fileName)
        {
            var file = await StorageFile.GetFileFromPathAsync(fileName);

            return await FileIO.ReadBufferAsync(file);
        }
    }
}
