using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.MediaProperties;

namespace Goblintools
{
    public class VideoCreator2
    {
        public MediaStreamSource Source { get; set; }

        public VideoCreator2()
        {
            var properties = VideoEncodingProperties.CreateUncompressed(CodecSubtypes.VideoFormatH265, 1002, 668);
            var descriptor = new VideoStreamDescriptor(properties);

            Source = new MediaStreamSource(descriptor);
            Source.SampleRequested += Source_SampleRequested;
        }

        private void Source_SampleRequested(MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            var buffer = new byte[] { }.AsBuffer();
            //args.Request.Sample = MediaStreamSample.CreateFromBuffer(buffer, );
        }
    }
}
