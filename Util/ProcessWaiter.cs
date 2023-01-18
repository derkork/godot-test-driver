using Godot;

namespace GodotTestDriver.Util
{
    internal partial class ProcessWaiter : Node
    {
        [Signal]
        public delegate void OnProcessEventHandler();

        [Signal]
        public delegate void OnPhysicsProcessEventHandler();


        public int CountDown { get; set; } = 1;

        public override void _PhysicsProcess(double delta)
        {
            if (GetSignalConnectionList(nameof(OnPhysicsProcess)).Count <= 0)
            {
                return;
            }

            CountDown--;
            if (CountDown > 0)
            {
                return;
            }

            EmitSignal(nameof(OnPhysicsProcess));
            Destroy();
        }

        private void Destroy()
        {
            GetParent().RemoveChild(this);
            QueueFree();
        }

        public override void _Process(double delta)
        {
            if (GetSignalConnectionList(nameof(OnProcess)).Count <= 0)
            {
                return;
            }

            CountDown--;
            if (CountDown > 0)
            {
                return;
            }

            EmitSignal(nameof(OnProcess));
            Destroy();
        }
    }
}