using System;
using Godot;
using GodotTestDriver.Drivers;

namespace YoHDot.Tests.TestDrivers.BuiltInNodes
{
    public class TweenDriver : NodeDriver<Tween>
    {
        public TweenDriver(Func<Tween> producer) : base(producer)
        {
        }

        public bool IsRunningAnimations => PresentRoot.IsActive();
    }
}