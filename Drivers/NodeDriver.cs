using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// The description given to the test driver.
        /// </summary>
        public string Description { get; }


        protected NodeDriver(Func<T> producer, string description = "")
        {
            _producer = producer;
            Description = description;
        }

        /// <summary>
        /// Is the node currently present in the tree?
        /// </summary>
        public bool IsPresent => Root != null;

        /// <summary>
        /// Builds drivers for a set of children of the current driver's root node. The first function needs to return
        /// the currently applicable child nodes, the second function will produce a driver for each child.
        /// </summary>
        protected IEnumerable<TDriver> BuildDrivers<TDriver, TNode>(Func<T, IEnumerable<TNode>> childSelector,
            Func<Func<TNode>, TDriver> driverFactory)
            where TDriver : NodeDriver where TNode:Node
        {
            var root = Root;
            if (root == null)
            {
                yield return null;
            }

            var children = childSelector(root).ToList();
            foreach (var child in children)
            {
                yield return driverFactory(() => Root?.GetNode<TNode>(child.GetPath()));
            }
        }

        /// <summary>
        /// Helper function to build an error message. Prefixes the message with a human readable description of this
        /// driver.
        /// </summary>
        protected string ErrorMessage(string message)
        {
            // if description is blank or empty, use the type name as description
            var typeName = GetType().Name;
            if (string.IsNullOrEmpty(Description))
            {
                return $"{typeName}: {message}";
            }

            return $"{typeName} [{Description}] {message}";
        }


        /// <summary>
        /// Returns the root node. Can be null in case the root
        /// node is not currently present in the scene tree or if the root node is not a valid instance anymore.
        /// </summary>
        [CanBeNull]
        public T Root
        {
            get
            {
                var node = _producer();
                return Object.IsInstanceValid(node) && node.IsInsideTree() ? node : null;
            }
        }

        public override Node RootNode => Root;

        /// <summary>
        /// Returns the root node and ensures it is present.
        /// </summary>
        public T PresentRoot
        {
            get
            {
                var result = Root;
                if (result == null)
                {
                    throw new InvalidOperationException(ErrorMessage("Node is not present in the scene tree."));
                }

                return result;
            }
        }

        /// <summary>
        /// Returns whether this node has the given signal connected to the given target.
        /// </summary>
        public bool IsSignalConnected(string signal, Object target, string method)
        {
            return PresentRoot.IsConnected(signal, target, method);
        }

        /// <summary>
        /// Returns whether the given signal of this node is connected to any other node.
        /// </summary>
        public bool IsSignalConnectedToAnywhere(string signal)
        {
            return PresentRoot.GetSignalConnectionList(signal).Count > 0;
        }
    }
}