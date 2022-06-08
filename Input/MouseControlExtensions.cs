using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;

namespace GodotTestDriver.Input
{
    /// <summary>
    /// Extension functionality for controlling the mouse from tests. 
    /// </summary>
    public static class MouseControlExtensions
    {
        public static async Task ClickMouseAt(this Viewport viewport, Vector2 position, ButtonList button = ButtonList.Left)
        {
            await viewport.PressMouseAt(position, button);
            await viewport.ReleaseMouseAt(position, button);
            // clicks almost always trigger something and waiting a frame gives the UI time to update
            // this way we don't need to litter our tests with waits.
            await viewport.NextFrame();
        }

        public static async Task MoveMouseTo(this Viewport viewport, Vector2 position)
        {
            viewport.WarpMouse(position); 
            await viewport.NextFrame();
        }

        public static async Task PressMouse(this Viewport viewport, ButtonList button = ButtonList.Left)
        {
            var action = new InputEventMouseButton();
            action.ButtonIndex = (int) button;
            action.Pressed = true;
            Godot.Input.ParseInputEvent(action);
            await viewport.NextFrame();
        }

        public static async Task ReleaseMouse(this Viewport viewport, ButtonList button = ButtonList.Left)
        {
            var action = new InputEventMouseButton();
            action.ButtonIndex = (int) button;
            action.Pressed = false;
            Godot.Input.ParseInputEvent(action);
            await viewport.NextFrame();
        }

        private static async Task PressMouseAt(this Viewport viewport, Vector2 position, ButtonList button = ButtonList.Left)
        {
            await MoveMouseTo(viewport, position);
            var action = new InputEventMouseButton();
            action.ButtonIndex = (int) button;
            action.Pressed = true;
            action.Position = position;
            Godot.Input.ParseInputEvent(action);
            await viewport.NextFrame();
        }

        private static async Task ReleaseMouseAt(this Viewport viewport, Vector2 position, ButtonList button = ButtonList.Left)
        {
            await MoveMouseTo(viewport, position);

            var action = new InputEventMouseButton();
            action.ButtonIndex = (int) button;
            action.Pressed = false;
            action.Position = position;
            Godot.Input.ParseInputEvent(action);
            await viewport.NextFrame();
        }
    }
}