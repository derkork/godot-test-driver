using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver.Input
{
    [PublicAPI]
    public static class ActionsControlExtensions
    {
        public static async Task HoldActionFor(
            this Node node,
            float seconds,
            string actionName
        )
        {
            await node.StartAction(actionName);
            await node.Wait(seconds);
            await node.EndAction(actionName);
        }

        public static async Task StartAction(this Node node, string actionName)
        {
            Godot.Input.ParseInputEvent(new InputEventAction
            {
                Action = actionName,
                Pressed = true
            });
            await node.GetTree().WaitForEvents();
        }

        public static async Task EndAction(this Node node, string actionName)
        {
            Godot.Input.ParseInputEvent(new InputEventAction
            {
                Action = actionName,
                Pressed = false
            });
            await node.GetTree().WaitForEvents();
        }

        public static async Task TriggerAction(this Node node, string actionName)
        {
            await node.StartAction(actionName);
            await node.EndAction(actionName);
        }
    }
}
