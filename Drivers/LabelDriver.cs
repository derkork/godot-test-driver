using System;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for the <see cref="Label"/> control.
    /// </summary>
    [PublicAPI]
    public class LabelDriver : ControlDriver<Label>
    {
        public LabelDriver(Func<Label> producer) : base(producer)
        {
        }

        public string Text => Root?.Text;
    }
}