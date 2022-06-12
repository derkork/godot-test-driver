using System;
using System.Threading.Tasks;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for a UI button.
    /// </summary>
    [PublicAPI]
    public class ButtonDriver : ControlDriver<Button>
    {
        public ButtonDriver(Func<Button> producer) : base(producer)
        {
        }

        public bool Disabled => Root is {Disabled: true};
        public bool Enabled => Root is {Disabled: false};

        /// <summary>
        ///  Simulates a button press by simply sending the press event.
        /// </summary>
        public void Press()
        {
            var button = VisibleRoot;
            if (button.Disabled)
            {
                throw new InvalidOperationException("Button is disabled and cannot be pressed.");
            }

            button.EmitSignal("pressed");
        }

        public override async Task ClickCenter(ButtonList button = ButtonList.Left)
        {
            if (Disabled)
            {
                throw new InvalidOperationException("Button is disabled and cannot be pressed.");
            }

            await base.ClickCenter(button);
        }
    }
}