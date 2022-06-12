using System;
using Godot;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Base class for test drivers that work on nodes. This is the non-generic variant, which will make it easier to
    /// create some fluent APIs.
    /// </summary>
    public abstract class NodeDriver
    {
        /// <summary>
        /// The root node that the test driver is working on.
        /// </summary>
        public abstract Node RootNode { get; }
        
    }
    
    /// <summary>
    /// Base class for test drivers that work on nodes.
    /// </summary>
    [PublicAPI]
    public abstract class NodeDriver<T> : NodeDriver where T : Node
    {
        private readonly Func<T> _producer;

        protected NodeDriver(Func<T> producer)
        {
            _producer = producer;
        }

        /// <summary>
        /// Is the node currently present in the tree?
        /// </summary>
        public bool IsPresent
        {
            get
            {
                var root = Root;
                return root != null && Object.IsInstanceValid(root);
            }
        }

        /// <summary>
        /// Returns the root node. Can be null in case the root
        /// node is not currently present in the scene tree or invalid.
        /// </summary>
        [CanBeNull]
        public T Root
        {
            get
            {
                var node =  _producer();
                return Object.IsInstanceValid(node) ? node : null;
            }
        }

        public override Node RootNode => Root;

        /// <summary>
        /// Returns the root node and ensures it is present.
        /// </summary>
        public T PresentRoot {
            get
            {
                var result = Root;
                if (result == null) {
                    throw new InvalidOperationException("Node is not present in the scene tree.");
                }
                return result;
            }
        }

        /// <summary>
        /// Creates a signal awaiter that waits for the given signal
        /// of the root node.
        /// </summary>
        protected SignalAwaiter GetSignalAwaiter(string signalName)
        {
            return PresentRoot.ToSignal(Root, signalName);
        }

        /// <summary>
        /// Returns whether this node has the given signal connected to the given target.
        /// </summary>
        public bool IsSignalConnected(string signal, Object target, string method)
        {
            return Root?.IsConnected(signal, target, method) ?? false;
        }

        /// <summary>
        /// Returns whether the given signal of this node is connected to any other node.
        /// </summary>
        public bool IsSignalConnectedToAnywhere(string signal)
        {
            return (Root?.GetSignalConnectionList(signal).Count ?? 0) > 0;
        }
        
        
        
    }
}