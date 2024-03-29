using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver.Input
{
    /// <summary>
    /// Extension functionality for controlling the mouse from tests. 
    /// </summary>
    [PublicAPI]
    public static class MouseControlExtensions
    {
        public static async Task ClickMouseAt(this Viewport viewport, Vector2 position, MouseButton button = MouseButton.Left)
        {
            await viewport.PressMouseAt(position, button);
            await viewport.ReleaseMouseAt(position, button);
        }

       
        public static async Task MoveMouseTo(this Viewport viewport, Vector2 position)
        {
            await viewport.ProcessFrame();
            
            viewport.WarpMouse(position);
            var inputEvent = new InputEventMouseMotion();
            inputEvent.GlobalPosition = position;
            inputEvent.Position = position;
            Godot.Input.ParseInputEvent(inputEvent);
            
            await viewport.WaitForEvents();
        }

        public static async Task DragMouse(this Viewport viewport, Vector2 start, Vector2 end, MouseButton button = MouseButton.Left)
        {
            await viewport.PressMouseAt(start, button);
            await viewport.ReleaseMouseAt(end, button);
        }

        public static async Task PressMouse(this Viewport viewport, MouseButton button = MouseButton.Left)
        {
            await viewport.ProcessFrame();
           
            var action = new InputEventMouseButton();
            action.ButtonIndex = button;
            action.Pressed = true;
            Godot.Input.ParseInputEvent(action);
            
            await viewport.WaitForEvents();
        }

        public static async Task ReleaseMouse(this Viewport viewport, MouseButton button = MouseButton.Left)
        {
            await viewport.ProcessFrame();
          
            var action = new InputEventMouseButton();
            action.ButtonIndex = button;
            action.Pressed = false;
            Godot.Input.ParseInputEvent(action);
            
            await viewport.WaitForEvents();
        }

        private static async Task PressMouseAt(this Viewport viewport, Vector2 position, MouseButton button = MouseButton.Left)
        {
            await MoveMouseTo(viewport, position);
          
            var action = new InputEventMouseButton();
            action.ButtonIndex = button;
            action.Pressed = true;
            action.Position = position;
            Godot.Input.ParseInputEvent(action);
         
            await viewport.WaitForEvents();
        }

        private static async Task ReleaseMouseAt(this Viewport viewport, Vector2 position, MouseButton button = MouseButton.Left)
        {
            await MoveMouseTo(viewport, position);
            
            var action = new InputEventMouseButton();
            action.ButtonIndex =  button;
            action.Pressed = false;
            action.Position = position;
            Godot.Input.ParseInputEvent(action);

            await viewport.WaitForEvents();
        }
    }
}