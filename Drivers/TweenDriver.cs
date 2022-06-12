using System;
using Godot;

namespace GodotTestDriver.Drivers
{
    public class TweenDriver : NodeDriver<Tween>
    {
        public TweenDriver(Func<Tween> producer) : base(producer)
        {
        }

        public bool IsRunningAnimations => Root?.IsActive() ?? false;
    }
}