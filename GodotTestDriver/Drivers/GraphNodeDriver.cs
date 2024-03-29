using System;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Input;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for a <see cref="GraphNode"/>
    /// </summary>
    [PublicAPI]
    public partial class GraphNodeDriver<T> : ControlDriver<T> where T:GraphNode
    {
        public GraphNodeDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }

        /// <summary>
        /// The title of the node
        /// </summary>
        public string Title => PresentRoot.Title;
        
        /// <summary>
        /// The offset of the node inside the graph.
        /// </summary>
        public Vector2 Offset => PresentRoot.PositionOffset;
        
        /// <summary>
        /// The relative position and size of the node.
        /// </summary>
        public Rect2 Rect => PresentRoot.GetRect();
        
        /// <summary>
        /// Whether the node is currently selected.
        /// </summary>
        public bool Selected => PresentRoot.Selected;

        /// <summary>
        /// The amount of input ports the node has.
        /// </summary>
        public int InputPortCount => PresentRoot.GetConnectionInputCount();
        
        /// <summary>
        /// The amount of output ports the node has.
        /// </summary>
        public int OutputPortCount => PresentRoot.GetConnectionOutputCount();

        
        /// <summary>
        /// Returns the port type of the given port.
        /// </summary>
        public int GetPortType(Port port)
        {
            if (!port.IsDefined)
            {
                throw new ArgumentException("Port is not defined.");
            }
            
            if (port.IsInput)
            {
                if (InputPortCount < port.PortIndex)
                {
                    throw new ArgumentException("Port index is out of range.");
                }
                return PresentRoot.GetConnectionInputType(port.PortIndex);
            }
            else
            {
                if (OutputPortCount < port.PortIndex)
                {
                    throw new ArgumentException("Port index is out of range.");
                }
                return PresentRoot.GetConnectionOutputType(port.PortIndex);
            }
        }
        
        /// <summary>
        /// Global position of a spot where the node can be safely clicked and dragged. Will fail if the node is not visible.
        /// </summary>
        protected virtual Vector2 SelectionSpot
        {
            get
            {
                var node = VisibleRoot;
                var rect = node.GetGlobalRect();
                // we assume that a position 5 pixels below the top border horizontally centered is a safe selection
                // spot as the node will have a title there.
                return rect.Position + new Vector2(rect.Size.X / 2, 5);
            }
        }

        /// <summary>
        /// Drags the node by the given amount of pixels.
        /// </summary>
        public async Task DragBy(Vector2 delta)
        {
            var dragStart = SelectionSpot;
            var dragEnd = dragStart + delta;

            // drag it
            await Viewport.DragMouse(dragStart, dragEnd);
        }

        /// <summary>
        /// Drags the node by the given amount of pixels.
        /// </summary>
        public async Task DragBy(float x, float y)
        {
            await DragBy(new Vector2(x, y));
        }

        /// <summary>
        /// Drags the node by a multiple of its own size multiplied by the given factor.
        /// </summary>
        public async Task DragByOwnSize(Vector2 delta)
        {
            var node = VisibleRoot;
            var rect = node.GetRect();

            await DragBy(new Vector2(rect.Size.X * delta.X, rect.Size.X * delta.Y));
        }

        /// <summary>
        /// Drags the node by a multiple of its own size multiplied by the given factor.
        /// </summary>
        public async Task DragByOwnSize(float x, float y)
        {
            await DragByOwnSize(new Vector2(x, y));
        }


        /// <summary>
        /// Selects the given node by clicking on it. Same as <see cref="ClickAtSelectionSpot"/>.
        /// </summary>
        public async Task Select()
        {
            await ClickAtSelectionSpot();
        }

        /// <summary>
        /// Clicks the mouse at the safe selection spot of this graph node.
        /// </summary>
        public async Task ClickAtSelectionSpot(MouseButton button = MouseButton.Left)
        {
            await Viewport.ClickMouseAt(SelectionSpot, button);
        }

        /// <summary>
        /// Drags a connection from the given source port of this node to the given target port of the given target node.
        /// </summary>
        public async Task DragConnection(Port sourcePort, GraphNodeDriver<T> targetNode, Port targetPort)
        {
            if (!sourcePort.IsDefined)
            {
                throw new ArgumentException("Source port is not defined.");
            }

            if (!targetPort.IsDefined)
            {
                throw new ArgumentException("Target port is not defined.");
            }

            var thisRoot = VisibleRoot;
            var targetRoot = targetNode.VisibleRoot;

            if (sourcePort.IsInput && sourcePort.PortIndex >= thisRoot.GetConnectionInputCount())
            {
                throw new ArgumentException($"Node has no input port at the given index {sourcePort.PortIndex}.");
            }

            if (sourcePort.IsOutput && sourcePort.PortIndex >= thisRoot.GetConnectionOutputCount())
            {
                throw new ArgumentException($"Node has no output port at the given index {sourcePort.PortIndex}.");
            }

            if (targetPort.IsInput && targetPort.PortIndex >= targetRoot.GetConnectionInputCount())
            {
                throw new ArgumentException(
                    $"Target node has no input port at the given index {targetPort.PortIndex}.");
            }

            if (targetPort.IsOutput && targetPort.PortIndex >= targetRoot.GetConnectionOutputCount())
            {
                throw new ArgumentException(
                    $"Target node has no output port at the given index {targetPort.PortIndex}.");
            }

            var startPosition = sourcePort.IsInput
                ? thisRoot.GetConnectionInputPosition(sourcePort.PortIndex)
                : thisRoot.GetConnectionOutputPosition(sourcePort.PortIndex);
            var endPosition = targetPort.IsInput
                ? targetRoot.GetConnectionInputPosition(targetPort.PortIndex)
                : targetRoot.GetConnectionOutputPosition(targetPort.PortIndex);

            await Viewport.DragMouse(startPosition + thisRoot.GlobalPosition,
                endPosition + targetRoot.GlobalPosition);
        }

        /// <summary>
        /// Drags a connection from the given source port of this node to a position relative to this port.
        /// </summary>
        public async Task DragConnection(Port sourcePort, Vector2 relativePosition)
        {
            if (!sourcePort.IsDefined)
            {
                throw new ArgumentException("Source port is not defined.");
            }

            var thisRoot = VisibleRoot;

            if (sourcePort.IsInput && sourcePort.PortIndex >= thisRoot.GetConnectionInputCount())
            {
                throw new ArgumentException($"Node has no input port at the given index {sourcePort.PortIndex}.");
            }

            if (sourcePort.IsOutput && sourcePort.PortIndex >= thisRoot.GetConnectionOutputCount())
            {
                throw new ArgumentException($"Node has no output port at the given index {sourcePort.PortIndex}.");
            }

            var startPosition = sourcePort.IsInput
                ? thisRoot.GetConnectionInputPosition(sourcePort.PortIndex)
                : thisRoot.GetConnectionOutputPosition(sourcePort.PortIndex);
            var endPosition = startPosition + relativePosition;

            await Viewport.DragMouse(startPosition + thisRoot.GlobalPosition,
                endPosition + thisRoot.GlobalPosition);
        }
    }
   
    /// <summary>
    /// Driver for a <see cref="GraphNode"/>
    /// </summary>
    [PublicAPI]
    public sealed class GraphNodeDriver : GraphNodeDriver<GraphNode>
    {
        public GraphNodeDriver(Func<GraphNode> producer, string description = "") : base(producer, description)
        {
        }
    }
}