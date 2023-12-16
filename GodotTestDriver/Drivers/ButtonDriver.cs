namespace GodotTestDriver.Drivers;

using System;
using Godot;
using JetBrains.Annotations;

/// <summary>
/// Driver for <see cref="Button"/> controls.
/// </summary>
/// <typeparam name="T">Button type.</typeparam>
[PublicAPI]
public class ButtonDriver<T> : BaseButtonDriver<T> where T : Button
{
    public ButtonDriver(Func<T> producer, string description = "") : base(producer, description)
    {
    }
}

/// <summary>
/// Driver for <see cref="Button"/> controls.
/// </summary>
[PublicAPI]
public sealed class ButtonDriver : ButtonDriver<Button>
{
    public ButtonDriver(Func<Button> producer, string description = "") : base(producer, description)
    {
    }
}
