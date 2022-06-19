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
    public class LineEditDriver<T> : ControlDriver<T> where T:LineEdit
    {
        public LineEditDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }


        public string Text => PresentRoot.Text;
        public bool Editable => PresentRoot.Editable;
        
        
        public async Task Type(string text)
        {
            if (!Editable)
            {
                throw new InvalidOperationException(ErrorMessage("Cannot type text into LineEdit because it is not editable."));
            }
            
            var edit = VisibleRoot;
            await edit.GetTree().ProcessFrame();
            await ClickCenter();
            edit.Text = text;
            edit.EmitSignal("text_changed", text);
            await edit.GetTree().WaitForEvents();
        }

        
        public async Task Enter(string text)
        {
            if (!Editable)
            {
                throw new InvalidOperationException(ErrorMessage("Cannot type text into LineEdit because it is not editable."));
            }
            
            var edit = VisibleRoot;
            // first type the text, so the text change events are triggered
            await Type(text);
            // then send the "text_entered" event 
            edit.EmitSignal("text_entered", text);
            await edit.GetTree().WaitForEvents();
        }
        
    }
    
    
    [PublicAPI]
    public sealed class LineEditDriver : LineEditDriver<LineEdit>
    {
        public LineEditDriver(Func<LineEdit> producer, string description = "") : base(producer, description)
        {
        }
    }
}