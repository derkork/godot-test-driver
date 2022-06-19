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
        public static Vector2 Center(this Rect2 rect2)
        {
            return rect2.Position + rect2.Size / 2;
        }
        
        [MustUseReturnValue]
        public static SignalAwaiter Sleep(this Node source, float sleepTime)
        {
            return source.ToSignal(source.GetTree().CreateTimer(sleepTime), "timeout");
        }

    }
}