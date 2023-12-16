namespace GodotTestDriver.Drivers;

using System;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

/// <summary>
/// Driver for <see cref="BaseButton"/> controls.
/// </summary>
/// <typeparam name="T">BaseButton type.</typeparam>
[PublicAPI]
public class BaseButtonDriver<T> : ControlDriver<T> where T : BaseButton
{
    public BaseButtonDriver(Func<T> producer, string description = "") : base(producer, description)
    {
    }

    /// <summary>
    /// Whether the button is currently disabled.
    /// </summary>
    public bool Disabled => PresentRoot.Disabled;

    /// <summary>
    /// Whether the button is currently enabled. Inverted from <see cref="Disabled"/>.
    /// </summary>
    public bool Enabled => !Disabled;

    /// <summary>
    /// Whether the button is currently pressed.
    /// </summary>
    public bool Pressed => PresentRoot.ButtonPressed;

    /// <summary>
    ///  Simulates a button press by simply sending the press event.
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    public async Task Press()
    {
        var button = VisibleRoot;

        if (button.Disabled)
        {
            throw new InvalidOperationException(ErrorMessage("Button is disabled and cannot be pressed."));
        }

        // make sure we run on main thread
        await button.GetTree().NextFrame();
        button.EmitSignal(BaseButton.SignalName.Pressed);
        await button.GetTree().WaitForEvents();
    }

    public override async Task ClickCenter(MouseButton button = MouseButton.Left)
    {
        if (Disabled)
        {
            throw new InvalidOperationException(ErrorMessage("Button is disabled and cannot be pressed."));
        }

        await base.ClickCenter(button);
    }
}
