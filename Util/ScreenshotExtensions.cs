using System;
using System.Globalization;
using System.Threading.Tasks;
using Godot;

namespace GodotTestDriver.Util
{
    /// <summary>
    /// Extension on <see cref="Viewport"/> to create screenshots during testing. The image is saved in PNG format.
    /// WARNING: this is currently experimental and not fully tested. It may disappear or change in the future.
    /// </summary>
    public static class ScreenshotExtensions
    {
        /// <summary>
        /// Takes a screenshot of the given viewport and saves it to the given file path. 
        /// </summary>
        public static async Task<Error> TakeScreenshot(this Viewport viewport, string filePath)
        {
            // save the previous clear mode so we can restore it after taking the screenshot
            var clearMode = viewport.RenderTargetClearMode;
            
            // clear the viewport for one frame so we make sure that _everything_ is redrawn.
            viewport.RenderTargetClearMode = Viewport.ClearMode.OnlyNextFrame;

            // wait until the visual server has re-drawn
            await viewport.ToSignal(VisualServer.Singleton, "frame_post_draw");
            
            // retrieve the captured image
            var image = viewport.GetTexture().GetData();
            
            // it is upside down, so flip it
            image.FlipY();

            // save it
            var result = image.SavePng(filePath);
            
            // restore the previous clear mode
            viewport.RenderTargetClearMode = clearMode;
            
            return result;
        }


        public static ArmedScreenshotCamera ArmScreenshotCamera(this Viewport viewport, string contextName,
            string filePathPrefix = "res://_test_screenshots")
        {
            return new ArmedScreenshotCamera(viewport, contextName, filePathPrefix);
        }


        public class ArmedScreenshotCamera
        {
            private readonly string _filePathPrefix;
            private readonly Viewport _viewport;
            private readonly string _contextName;
            private int _frameCount;


            internal ArmedScreenshotCamera(Viewport viewport, string contextName, string filePathPrefix)
            {
                _filePathPrefix = filePathPrefix;
                _viewport = viewport;
                _contextName = contextName;
            }
            
            public async Task<Error> TakeScreenshot()
            {
                _frameCount += 1;
                // ReSharper disable once StringLiteralTypo
                var fileName = string.Format(CultureInfo.InvariantCulture, "Screenshot-{0}-{1:yyyyMMddHHmmss}-{2}.png",
                    _contextName.Left(98), DateTime.Now, _frameCount); 
                var filePath = $"{_filePathPrefix}/{fileName}";
                return await _viewport.TakeScreenshot(filePath);
            }
        }
        
    }
}