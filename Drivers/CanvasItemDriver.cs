using System;
using System.Diagnostics;
using Godot;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for <see cref="CanvasItem"/> nodes.
    /// </summary>
    [PublicAPI]
    public class CanvasItemDriver<T> : NodeDriver<T> where T : CanvasItem
    {
        public CanvasItemDriver(Func<T> producer) : base(producer)
        {
        }

        /// <summary>
        /// Is the CanvasItem currently visible?
        /// </summary>
        public bool IsVisible
        {
            get
            {
                var node = Root;
                return node != null && Object.IsInstanceValid(node) && node.IsVisibleInTree();
            }
        }

        /// <summary>
        /// The viewport that this canvas item is rendering to.
        /// </summary>
        public Viewport Viewport => PresentRoot.GetViewport();
        
        /// <summary>
        /// Returns the root node but ensures it is visible.
        /// </summary>
        protected T VisibleRoot
        {
            get
            {
                var root = Root;
                if (root == null ||  !Object.IsInstanceValid(root) || !root.IsVisibleInTree())
                {
                    throw new InvalidOperationException("Cannot interact with CanvasItem because it is not visible.");
                }
                return root;
            }
        }
    }
}