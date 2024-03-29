﻿using System;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Input;
using GodotTestDriver.Util;
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
        /// Is the window currently visible?
        /// </summary>
        public bool IsVisible => PresentRoot.Visible;

        /// <summary>
        /// The viewport that this window item is rendering to.
        /// </summary>
        public Viewport Viewport => PresentRoot.GetViewport();
        
        /// <summary>
        /// The position of the window in pixels.
        /// </summary>
        public Vector2I Position => PresentRoot.Position;
        
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
        
        
        /// <summary>
        /// Closes the window by clicking the close button.
        /// </summary>
        public async Task Close()
        {
            var window = VisibleRoot;
            await window.GetTree().NextFrame();
            window.EmitSignal(Window.SignalName.CloseRequested);
            await window.GetTree().WaitForEvents();
        }

        /// <summary>
        /// Drags the window by the given offset.
        /// </summary>
        public async Task DragByPixels(int x, int y)
        {
            await DragByPixels(new Vector2I(x, y));
        }
        
        
        /// <summary>
        /// Drags the window by the given offset.
        /// </summary>
        public async Task DragByPixels(Vector2I offset)
        {
            var window = VisibleRoot;
            
            // check that the window has a parent otherwise we can't drag it
            if (window.GetParent() == null)
            {
                throw new InvalidOperationException(ErrorMessage("Dragging of root windows is not supported."));
            }
            
            await window.GetTree().NextFrame();

            var titleBarHeight = window.GetThemeConstant("title_height");
            
            // get position and width of window, then use the title bar height to get the center of the title bar
            // note that the title bar is ABOVE the window, so we need to subtract the title bar height from
            // the window's position to get the top of the title bar
            var pos = window.Position;
            var width = window.Size.X;
            var startSpot = new Vector2(pos.X + width / 2f,  pos.Y - titleBarHeight / 2f);
            var endSpot = startSpot + offset;
            
            

            await window.GetParent().GetViewport().DragMouse(startSpot, endSpot);
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