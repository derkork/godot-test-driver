using System;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for <see cref="Node2D"/> nodes.
    /// </summary>
    [PublicAPI]
    public class Node2DDriver<T> : CanvasItemDriver<T> where T : Node2D
    {
        public Node2DDriver(Func<T> producer) : base(producer)
        {
        }
        

        public Vector2 GlobalPosition => PresentRoot.GlobalPosition;

    }
}