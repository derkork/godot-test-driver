using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Util
{
    /// <summary>
    /// Some useful extension methods. These are declared internal instead using GodotExt, so users can decide what
    /// they want.
    /// </summary>
    internal static class InternalUtil
    {
        /// <summary>
        /// Returns the center of the given rect.
        /// </summary>
        public static Vector2 Center(this Rect2 rect2)
        {
            return rect2.Position + rect2.Size / 2;
        }

        /// <summary>
        /// Sleeps for the given amount of seconds.
        /// </summary>
        [MustUseReturnValue]
        public static SignalAwaiter SleepSeconds(this Node source, float sleepTime)
        {
            return source.ToSignal(source.GetTree().CreateTimer(sleepTime), SceneTreeTimer.SignalName.Timeout);
        }

        /// <summary>
        /// Converts the given rect to global coordinates.
        /// </summary>
        public static Rect2 ToGlobalRect(this Rect2 localRect, Vector2 globalPosition)
        {
            return new Rect2(globalPosition + localRect.Position, localRect.Size);
        }
        
        /// <summary>
        /// Converts the given world position to viewport screen coordinates.
        /// </summary>
        public static Vector2 WorldToScreen(this Viewport viewport, Vector2 worldPosition)
        {
            return viewport.CanvasTransform * worldPosition;
        }
        
        /// <summary>
        /// Converts the given viewport screen coordinates to world position.
        /// </summary>
        public static Vector2 ScreenToWorld(this Viewport viewport, Vector2 screenPosition)
        {
            return screenPosition * viewport.CanvasTransform;
        } 
        
        // same for Rect2
        
        /// <summary>
        /// Converts the given world rect to viewport screen coordinates.
        /// </summary>
        public static Rect2 WorldToScreen(this Viewport viewport, Rect2 worldRect)
        {
            return viewport.CanvasTransform * worldRect;
        }
        
        /// <summary>
        /// Converts the given viewport screen coordinates to world rect.
        /// </summary>
        public static Rect2 ScreenToWorld(this Viewport viewport, Rect2 screenRect)
        {
            return screenRect * viewport.CanvasTransform;
        }
    }
}