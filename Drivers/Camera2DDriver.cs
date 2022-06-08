using System;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for the Camera2D node.
    /// </summary>
    [PublicAPI]
    public class Camera2DDriver : NodeDriver<Camera2D>
    {
        public Camera2DDriver(Func<Camera2D> producer) : base(producer)
        {
        }

        public async Task<bool> MoveIntoView(Vector2 worldPosition, float timeoutSeconds)
        {
            PresentRoot.GlobalPosition = worldPosition;
            return await WaitUntilSteady(timeoutSeconds);
        }
        
        /// <summary>
        /// Waits until the camera steady (e.g. not moving for three frames).
        /// </summary>
        public async Task<bool> WaitUntilSteady(float timeoutSeconds)
        {
            var timeout = new Timeout(timeoutSeconds);
            var screenPos = PresentRoot.GetCameraScreenCenter();
            // we treat the camera as steady when it hasn't moved over 3 frames.
            var frameCount = 0;

            do
            {
                var newScreenPos = PresentRoot.GetCameraScreenCenter();
                if ((newScreenPos - screenPos).LengthSquared() < 0.001)
                {
                    frameCount++;
                }
                else
                {
                    frameCount = 0;
                }

                if (frameCount >= 3)
                {
                    return true;
                }

                screenPos = newScreenPos;
                await PresentRoot.NextFrame();
            } while (!timeout.IsReached);

            return false;
        }
    }
}