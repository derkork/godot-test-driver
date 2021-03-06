using System;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for <see cref="Tween"/> nodes.
    /// </summary>
    [PublicAPI]
    public class TweenDriver<T> : NodeDriver<T> where T : Tween
    {
        public TweenDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }

        /// <summary>
        /// Whether the tween is currently playing animations.
        /// </summary>
        public bool IsRunningAnimations => PresentRoot.IsActive();
    }

    /// <summary>
    /// Driver for <see cref="Tween"/> nodes.
    /// </summary>
    [PublicAPI]
    public sealed class TweenDriver : TweenDriver<Tween>
    {
        public TweenDriver(Func<Tween> producer, string description = "") : base(producer, description)
        {
        }
    }
}