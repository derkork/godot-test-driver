using System;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for the <see cref="RichTextLabel"/> control.
    /// </summary>
    [PublicAPI]
    public class RichTextLabelDriver : ControlDriver<RichTextLabel>
    {
        public RichTextLabelDriver(Func<RichTextLabel> producer) : base(producer)
        {
        }

        public string Text => Root?.Text;
        public string BbCodeText => Root?.BbcodeText;
    }
}