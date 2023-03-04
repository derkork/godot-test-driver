using System;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// A driver for UI Windows.
    /// </summary>
    [PublicAPI]
    public class WindowDriver<T> : NodeDriver<T> where T:Window
    {
        public WindowDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }
        
        
        /// <summary>
        /// Is the CanvasItem currently visible?
        /// </summary>
        public bool IsVisible => PresentRoot.Visible;

        /// <summary>
        /// The viewport that this window item is rendering to.
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
                if (!root.Visible)
                {
                    throw new InvalidOperationException(ErrorMessage("Cannot interact with window because it is not visible."));
                }
                return root;
            }
        }
    }
    
    [PublicAPI]
    public class WindowDriver : WindowDriver<Window>
    {
        public WindowDriver(Func<Window> producer, string description = "") : base(producer, description)
        {
        }
    }
}