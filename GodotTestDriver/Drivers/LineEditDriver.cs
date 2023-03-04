using System;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for the <see cref="LineEdit"/> control.
    /// </summary>
    [PublicAPI]
    public partial class LineEditDriver<T> : ControlDriver<T> where T:LineEdit
    {
        public LineEditDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }

        /// <summary>
        /// The current text of the line edit.
        /// </summary>
        public string Text => PresentRoot.Text;

        /// <summary>
        /// Whether the line edit is currently editable.
        /// </summary>
        public bool Editable => PresentRoot.Editable;
        
        /// <summary>
        /// Types the given text into the line edit. Existing text will be overwritten.
        /// </summary>
        public async Task Type(string text)
        {
            if (!Editable)
            {
                throw new InvalidOperationException(ErrorMessage("Cannot type text into LineEdit because it is not editable."));
            }
            
            var edit = VisibleRoot;
            await edit.GetTree().NextFrame();
            await ClickCenter();
            edit.Text = text;
            edit.EmitSignal(LineEdit.SignalName.TextChanged, text);
            await edit.GetTree().WaitForEvents();
        }


        /// <summary>
        /// Types the given text into the line edit. Existing text will be overwritten. Presses "enter" afterwards.
        /// </summary>
        public async Task Submit(string text)
        {
            if (!Editable)
            {
                throw new InvalidOperationException(ErrorMessage("Cannot type text into LineEdit because it is not editable."));
            }
            
            var edit = VisibleRoot;
            // first type the text, so the text change events are triggered
            await Type(text);
            // then send the "TextSubmitted" event 
            edit.EmitSignal(LineEdit.SignalName.TextSubmitted, text);
            await edit.GetTree().WaitForEvents();
        }
        
    }
    
    /// <summary>
    /// Driver for the <see cref="LineEdit"/> control.
    /// </summary>
    [PublicAPI]
    public sealed class LineEditDriver : LineEditDriver<LineEdit>
    {
        public LineEditDriver(Func<LineEdit> producer, string description = "") : base(producer, description)
        {
        }
    }
}