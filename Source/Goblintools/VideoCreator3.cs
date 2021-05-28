using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Editing;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace Goblintools
{
    public class VideoCreator3
    {
        //MediaEncodingProfile Profile { get; set; }

        public VideoCreator3()
        {
            //Profile = MediaEncodingProfile.CreateHevc(VideoEncodingQuality.Auto);


        }

        public async void Start()
        {
            var composition = new MediaComposition();
            var source = await StorageFile.GetFileFromPathAsync(@"C:\Source\GitHub\JVermeulen\Goblintools\Data\Maps\_classic_\947.png");
            var target = await StorageFile.GetFileFromPathAsync(@"C:\Source\GitHub\JVermeulen\Goblintools\Data\Results\test.mp4");
            var mediaClip = await MediaClip.CreateFromImageFileAsync(source, TimeSpan.FromSeconds(1));

            composition.Clips.Add(mediaClip);

            await composition.RenderToFileAsync(target, MediaTrimmingPreference.Fast);
        }
    }
}
