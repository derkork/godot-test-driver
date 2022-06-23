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
        public static async Task PressKey(this Node node, KeyList key, bool control = false, bool alt = false,
            bool shift = false, bool meta = false, bool command = false)
        {
            var inputEvent = new InputEventKey();
            inputEvent.Pressed = true;
            inputEvent.Scancode = (uint) key;
            inputEvent.Alt = alt;
            inputEvent.Control = control;
            inputEvent.Shift = shift;
            inputEvent.Meta = meta;
            inputEvent.Command = command;

            Godot.Input.ParseInputEvent(inputEvent);

            await node.WaitForEvents();
        }

        public static async Task PressShiftF1(this Node node)
        {
            await node.PressKey(KeyList.F1, shift: true, control: true);
        }

        /// <summary>
        /// Releases the given key with the given modifier state.
        /// </summary>
        public static async Task ReleaseKey(this Node node, KeyList key, bool control = false, bool alt = false,
            bool shift = false,
            bool meta = false, bool command = false)
        {
            var inputEvent = new InputEventKey();
            inputEvent.Pressed = false;
            inputEvent.Scancode = (uint) key;
            inputEvent.Alt = alt;
            inputEvent.Shift = shift;
            inputEvent.Meta = meta;
            inputEvent.Command = command;

            Godot.Input.ParseInputEvent(inputEvent);

            await node.WaitForEvents();
        }

        /// <summary>
        /// Presses and releases a key with the given modifiers.
        /// </summary>
        public static async Task TypeKey(this Node node, KeyList key, bool control = false, bool alt = false,
            bool shift = false, bool meta = false, bool command = false)
        {
            await node.PressKey(key, control, alt, shift, meta, command);
            await node.ReleaseKey(key, control, alt, shift, meta, command);
        }
    }
}