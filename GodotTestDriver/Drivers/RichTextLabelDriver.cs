using System;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for the <see cref="RichTextLabel"/> control.
    /// </summary>
    [PublicAPI]
    public partial class RichTextLabelDriver<T> : ControlDriver<T> where T:RichTextLabel
    {
        public RichTextLabelDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }

        /// <summary>
        /// The current text of the label.
        /// </summary>
        public string Text => PresentRoot.Text;

        
        /// <summary>
        /// Returns true if the label has bbcode enabled.
        /// </summary>
        public bool IsBbcodeEnabled => PresentRoot.BbcodeEnabled;
    }
 
    /// <summary>
    /// Driver for the <see cref="RichTextLabel"/> control.
    /// </summary>
    [PublicAPI]
    public sealed class RichTextLabelDriver : RichTextLabelDriver<RichTextLabel>
    {
        public RichTextLabelDriver(Func<RichTextLabel> producer, string description = "") : base(producer, description)
        {
        }
    }
}