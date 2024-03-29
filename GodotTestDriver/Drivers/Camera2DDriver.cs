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
    public partial class Camera2DDriver<T> : Node2DDriver<T> where T:Camera2D
    {
        public Camera2DDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }

        /// <summary>
        /// Moves the given position into the view of the camera. This will wait for the given amount of seconds
        /// until the camera no longer moves.
        /// </summary>
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
            var screenPos = PresentRoot.GetScreenCenterPosition();
            // we treat the camera as steady when it hasn't moved over 3 frames.
            var frameCount = 0;

            do
            {
                var newScreenPos = PresentRoot.GetScreenCenterPosition(); 
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
                await PresentRoot.GetTree().NextFrame();
            } while (!timeout.IsReached);

            return false;
        }
    }
   
    /// <summary>
    /// Driver for the Camera2D node.
    /// </summary>
    [PublicAPI]
    public sealed class Camera2DDriver : Camera2DDriver<Camera2D>
    {
        public Camera2DDriver(Func<Camera2D> producer, string description = "") : base(producer, description)
        {
        }
    }
}