using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver.Input
{
    /// <summary>
    /// Extensions which allow to send keyboard inputs.
    /// </summary>
    [PublicAPI]
    public static class KeyboardControlExtensions
    {
        /// <summary>
        /// Presses the given key with the given modifiers.
        /// </summary>
        public static async Task PressKey(this Node node, Key key, bool control = false, bool alt = false,
            bool shift = false, bool meta = false)
        {
            var inputEvent = new InputEventKey();
            inputEvent.Pressed = true;
            inputEvent.Keycode = key;
            inputEvent.AltPressed = alt;
            inputEvent.CtrlPressed = control;
            inputEvent.ShiftPressed = shift;
            inputEvent.MetaPressed = meta;

            Godot.Input.ParseInputEvent(inputEvent);

            await node.WaitForEvents();
        }

        /// <summary>
        /// Releases the given key with the given modifier state.
        /// </summary>
        public static async Task ReleaseKey(this Node node, Key key, bool control = false, bool alt = false,
            bool shift = false, bool meta = false)
        {
            var inputEvent = new InputEventKey();
            inputEvent.Pressed = false;
            inputEvent.Keycode =  key;
            inputEvent.CtrlPressed = control;
            inputEvent.AltPressed = alt;
            inputEvent.ShiftPressed = shift;
            inputEvent.MetaPressed = meta;

            Godot.Input.ParseInputEvent(inputEvent);

            await node.WaitForEvents();
        }

        /// <summary>
        /// Presses and releases a key with the given modifiers.
        /// </summary>
        public static async Task TypeKey(this Node node, Key key, bool control = false, bool alt = false,
            bool shift = false, bool meta = false)
        {
            await node.PressKey(key, control, alt, shift, meta);
            await node.ReleaseKey(key, control, alt, shift, meta);
        }
    }
}