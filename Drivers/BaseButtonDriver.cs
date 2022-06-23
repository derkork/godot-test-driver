using System;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for <see cref="BaseButton"/> controls.
    /// </summary>
    [PublicAPI]
    public class BaseButtonDriver<T> : ControlDriver<T> where T:BaseButton
    {
        public BaseButtonDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }

        /// <summary>
        /// Whether the button is currently disabled.
        /// </summary>
        public bool Disabled => PresentRoot.Disabled;
        
        /// <summary>
        /// Whether the button is currently enabled. Inversed from <see cref="Disabled"/>.
        /// </summary>
        public bool Enabled => !Disabled;
        
        /// <summary>
        /// Whether the button is currently pressed.
        /// </summary>
        public bool Pressed => PresentRoot.Pressed;

        /// <summary>
        ///  Simulates a button press by simply sending the press event.
        /// </summary>
        public async Task Press()
        {
            var button = VisibleRoot;

            if (button.Disabled)
            {
                throw new InvalidOperationException(ErrorMessage($"Button is disabled and cannot be pressed."));
            }

            // make sure we run on main thread
            await button.GetTree().ProcessFrame();
            button.EmitSignal("pressed");
            await button.GetTree().WaitForEvents();
        }

        public override async Task ClickCenter(ButtonList button = ButtonList.Left)
        {
            if (Disabled)
            {
                throw new InvalidOperationException(ErrorMessage("Button is disabled and cannot be pressed."));
            }

            await base.ClickCenter(button);
        }
    }
}