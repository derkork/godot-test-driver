namespace GodotTestDriver.Drivers;

using System;
using Godot;
using JetBrains.Annotations;

/// <summary>
/// Driver for a <see cref="CheckBox"/>.
/// </summary>
/// <typeparam name="T">CheckBox type.</typeparam>
[PublicAPI]
public class CheckBoxDriver<T> : ButtonDriver<T> where T : CheckBox
{
    public CheckBoxDriver(Func<T> provider, string description = "") : base(provider, description)
    {
    }

    public bool IsChecked => PresentRoot.ButtonPressed;
}

/// <summary>
/// Driver for a <see cref="CheckBox"/>.
/// </summary>
[PublicAPI]
public sealed class CheckBoxDriver : CheckBoxDriver<CheckBox>
{
    public CheckBoxDriver(Func<CheckBox> provider, string description = "") : base(provider, description)
    {
    }
}
