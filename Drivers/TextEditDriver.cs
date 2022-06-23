using System;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for the <see cref="TextEdit"/> control.
    /// </summary>
    [PublicAPI]
    public class TextEditDriver<T> : ControlDriver<T> where T : TextEdit
    {
        public TextEditDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }


        public string Text => PresentRoot.Text;
        public bool ReadOnly => PresentRoot.Readonly;


        public async Task Type(string text)
        {
            if (ReadOnly)
            {
                throw new InvalidOperationException(
                    ErrorMessage("Cannot type text into TextEdit because it is read-only."));
            }

            var edit = VisibleRoot;
            await edit.GetTree().ProcessFrame();
            await ClickCenter();
            edit.Text = text;
            edit.EmitSignal("text_changed", text);
            await edit.GetTree().WaitForEvents();
        }
    }

    /// <summary>
    /// Driver for the <see cref="TextEdit"/> control.
    /// </summary>  
    [PublicAPI]
    public class TextEditDriver : TextEditDriver<TextEdit>
    {
        public TextEditDriver(Func<TextEdit> producer, string description = "") : base(producer, description)
        {
        }
    }
}