using System;
using System.Threading.Tasks;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for the <see cref="LineEdit"/> control.
    /// </summary>
    [PublicAPI]
    public class LineEditDriver : ControlDriver<LineEdit>
    {
        public LineEditDriver(Func<LineEdit> producer) : base(producer)
        {
        }


        public string Text => PresentRoot.Text;
        
        public async Task Type(string text)
        {
            var edit = VisibleRoot;
            await ClickCenter();
            edit.Text = text;
            edit.EmitSignal("text_changed", text);
        }

        
        public async Task Enter(string text)
        {
            var edit = VisibleRoot;
            // first type the text, so the text change events are triggered
            await Type(text);
            // then send the "text_entered" event 
            edit.EmitSignal("text_entered", text);
        }
        
    }
}