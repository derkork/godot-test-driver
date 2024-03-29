using System;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for <see cref="CanvasItem"/> nodes.
    /// </summary>
    [PublicAPI]
    public partial class CanvasItemDriver<T> : NodeDriver<T> where T : CanvasItem
    {
        public CanvasItemDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }

        /// <summary>
        /// Is the CanvasItem currently visible?
        /// </summary>
        public bool IsVisible => PresentRoot.IsVisibleInTree();

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
                var root = PresentRoot;
                if (!root.IsVisibleInTree())
                {
                    throw new InvalidOperationException(ErrorMessage("Cannot interact with CanvasItem because it is not visible."));
                }
                return root;
            }
        }
    }
}