using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Editing;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace Goblintools.UwpApp
{
    public class VideoCreator3
    {
        private MediaComposition Composition { get; set; }
        public Dictionary<int, CanvasBitmap> Maps { get; set; }
        public int FPS { get; set; } = 25;
        public TimeSpan ClipDuration => TimeSpan.FromSeconds(1 / FPS);

        public VideoCreator3()
        {
            Composition = new MediaComposition();
        }

        public async Task<StorageFile> SelectDestination()
        {
            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            picker.FileTypeChoices.Add("MP4 files", new List<string>() { ".mp4" });
            picker.SuggestedFileName = "Pugnator.mp4";

            return await picker.PickSaveFileAsync();
        }

        public void Start()
        {
            StartAsync().Wait();
        }

        public async Task StartAsync()
        {
            await LoadMaps();
            //await AddClip(@"C:\Users\JoostVermeulenTensin\Pictures\Goblintools\947.png");
            //await AddClip(@"C:\Users\JoostVermeulenTensin\Pictures\Goblintools\1411.png");
            await AddMaps();

            var target = await SelectDestination();
            await Composition.RenderToFileAsync(target, MediaTrimmingPreference.Precise);
        }

        public async Task LoadMaps()
        {
            var keys = new int[] { 947, 1411 };

            Maps = new Dictionary<int, CanvasBitmap>();

            foreach (var key in keys)
            {
                var bitmap = await LoadFileAsCanvasBitmap($"C:\\Users\\JoostVermeulenTensin\\Pictures\\Goblintools\\{key}.png");

                Maps.Add(key, bitmap);
            }
        }

        public async Task<CanvasBitmap> LoadFileAsCanvasBitmap(string fileName)
        {
            var file = await StorageFile.GetFileFromPathAsync(fileName);

            using (var input = await file.OpenReadAsync())
            {
                return await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), input);
            }
        }

        public async Task<MediaClip> CreateMediaClip(int mapID)
        {
            var map = Maps[mapID];

            var rendertarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), map.SizeInPixels.Width, map.SizeInPixels.Height, 96);
            using (CanvasDrawingSession session = rendertarget.CreateDrawingSession())
            {
                session.Clear(Colors.Black);
                session.DrawImage(map);
            }

            return MediaClip.CreateFromSurface(rendertarget, ClipDuration);
        }

        public async Task<MediaClip> CreateMediaClip(string fileName)
        {
            var file = await StorageFile.GetFileFromPathAsync(fileName);
            //var background = await MediaClip.CreateFromImageFileAsync(file, TimeSpan.FromSeconds(1));

            CanvasRenderTarget rendertarget = null;

            using (var input = await file.OpenReadAsync())
            using (CanvasBitmap canvas = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), input))
            {
                rendertarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), canvas.SizeInPixels.Width, canvas.SizeInPixels.Height, 96);
                using (CanvasDrawingSession session = rendertarget.CreateDrawingSession())
                {
                    session.Clear(Colors.Black);
                    session.DrawImage(canvas);
                }
            }

            return MediaClip.CreateFromSurface(rendertarget, ClipDuration);
        }

        public async Task AddMaps()
        {
            for (int i = 0; i < 250; i++)
            {
                Composition.Clips.Add(await CreateMediaClip(947));
                Composition.Clips.Add(await CreateMediaClip(1411));
            }
        }

        public async Task AddClip(string fileName)
        {
            var file = await StorageFile.GetFileFromPathAsync(fileName);
            var clip = await MediaClip.CreateFromImageFileAsync(file, TimeSpan.FromSeconds(1));

            if (Composition.Clips.Count == 0)
            {
                Composition.Clips.Add(clip);
            }
            else
            {
                var overlay = CreateOverlay(clip, 0.5, 16, 16, 0.5);
                MediaOverlayLayer mediaOverlayLayer = new MediaOverlayLayer();
                mediaOverlayLayer.Overlays.Add(overlay);
                Composition.OverlayLayers.Add(mediaOverlayLayer);
            }
        }

        public MediaOverlay CreateOverlay(MediaClip overlayMediaClip, double scale, double left, double top, double opacity)
        {
            //Windows.Media.MediaProperties.VideoEncodingProperties encodingProperties = overlayMediaClip.GetVideoEncodingProperties();

            Rect overlayPosition;

            overlayPosition.Width = (double)256;
            overlayPosition.Height = (double)128;
            overlayPosition.X = left;
            overlayPosition.Y = top;

            MediaOverlay mediaOverlay = new MediaOverlay(overlayMediaClip);
            mediaOverlay.Position = overlayPosition;
            mediaOverlay.Opacity = opacity;

            return mediaOverlay;


        }
    }
}
