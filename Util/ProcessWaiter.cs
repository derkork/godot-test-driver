using Godot;

namespace GodotTestDriver.Util
{
    internal class ProcessWaiter : Node
    {
        [Signal]
        public delegate void OnProcess();

        [Signal]
        public delegate void OnPhysicsProcess();


        public int CountDown { get; set; } = 1;

        public override void _PhysicsProcess(float delta)
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

        public override void _Process(float delta)
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