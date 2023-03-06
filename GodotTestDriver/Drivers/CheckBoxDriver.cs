using System;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for a <see cref="CheckBox"/>.
    /// </summary>
    [PublicAPI]
    public partial class CheckBoxDriver<T> : ButtonDriver<T> where T:CheckBox
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
}