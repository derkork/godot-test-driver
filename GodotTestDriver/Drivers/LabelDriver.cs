using System;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for the <see cref="Label"/> control.
    /// </summary>
    [PublicAPI]
    public partial class LabelDriver<T> : ControlDriver<T> where T:Label
    {
        public LabelDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }

        /// <summary>
        /// The current text of the label.
        /// </summary>
        public string Text => PresentRoot.Text;
    }
    
    [PublicAPI]
    public sealed class LabelDriver : LabelDriver<Label>
    {
        public LabelDriver(Func<Label> producer, string description = "") : base(producer, description)
        {
        }
    }
}