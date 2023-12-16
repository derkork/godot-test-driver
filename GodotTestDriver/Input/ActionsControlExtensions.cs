namespace GodotTestDriver.Input;

using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

/// <summary>
/// Input action extensions.
/// </summary>
[PublicAPI]
public static class ActionsControlExtensions
{
    /// <summary>
    /// Hold an input action for a given duration.
    /// </summary>
    /// <param name="node">Node to supply input to.</param>
    /// <param name="seconds">Time, in seconds.</param>
    /// <param name="actionName">Name of the action.</param>
    /// <returns>Task that completes when the input finishes.</returns>
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

    /// <summary>
    /// Start an input action.
    /// </summary>
    /// <param name="node">Node to supply input to.</param>
    /// <param name="actionName">Name of the action.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static async Task StartAction(this Node node, string actionName)
    {
        Input.ParseInputEvent(new InputEventAction
        {
            Action = actionName,
            Pressed = true
        });
        await node.GetTree().WaitForEvents();
    }

    /// <summary>
    /// End an input action.
    /// </summary>
    /// <param name="node">Node to supply input to.</param>
    /// <param name="actionName">Name of the action.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static async Task EndAction(this Node node, string actionName)
    {
        Input.ParseInputEvent(new InputEventAction
        {
            Action = actionName,
            Pressed = false
        });
        await node.GetTree().WaitForEvents();
    }

    /// <summary>
    /// Trigger an input action by immediately starting and ending it.
    /// </summary>
    /// <param name="node">Node to supply input to.</param>
    /// <param name="actionName">Name of the action.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static async Task TriggerAction(this Node node, string actionName)
    {
        await node.StartAction(actionName);
        await node.EndAction(actionName);
    }
}
