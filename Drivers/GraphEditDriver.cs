using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for a <see cref="GraphEdit"/>.  
    /// </summary>
    [PublicAPI]
    public class GraphEditDriver<TGraphEdit, TGraphNodeDriver, TGraphNode> : ControlDriver<TGraphEdit>
        where TGraphEdit : GraphEdit where TGraphNode : GraphNode where TGraphNodeDriver : GraphNodeDriver<TGraphNode>
    {
        private readonly Func<Func<TGraphNode>, string, TGraphNodeDriver> _nodeDriverProducer;

        /// <summary>
        /// Constructs a new driver.
        /// </summary>
        /// <param name="producer">a producer that produces the <see cref="GraphEdit"/> that this driver works on.</param>
        /// <param name="nodeDriverProducer">a producer that produces a driver for a <see cref="GraphNode"/> child of the <see cref="GraphEdit"/></param>
        /// <param name="description">a description for the node</param>
        public GraphEditDriver(Func<TGraphEdit> producer,
            Func<Func<TGraphNode>, string, TGraphNodeDriver> nodeDriverProducer,
            string description = "") : base(producer, description)
        {
            _nodeDriverProducer = nodeDriverProducer;
        }

        /// <summary>
        /// Checks if the graph edit has a connection from the given node to the given target node on the
        /// given ports.
        /// </summary>
        public bool HasConnection(TGraphNodeDriver from, Port fromPort, TGraphNodeDriver to, Port toPort)
        {
            if (!fromPort.IsOutput)
            {
                throw new ArgumentException("fromPort must be an output port");
            }

            if (!toPort.IsInput)
            {
                throw new ArgumentException("toPort must be an input port");
            }

            var graphEdit = PresentRoot;
            var fromRoot = from.PresentRoot;
            var toRoot = to.PresentRoot;

            return graphEdit.GetConnectionList().Cast<Dictionary>()
                .Any(connection =>
                    (string) connection["from"] == fromRoot.Name
                    && (int) connection["from_port"] == fromPort.PortIndex
                    && (string) connection["to"] == toRoot.Name
                    && (int) connection["to_port"] == toPort.PortIndex);
        }

        /// <summary>
        /// Checks if he graph edit has a connection originating from the given node on the given port.
        /// </summary>
        public bool HasConnectionFrom(TGraphNodeDriver from, Port fromPort)
        {
            if (!fromPort.IsOutput)
            {
                throw new ArgumentException("fromPort must be an output port");
            }

            var graphEdit = PresentRoot;
            var fromRoot = from.PresentRoot;

            return graphEdit.GetConnectionList().Cast<Dictionary>()
                .Any(connection =>
                    (string) connection["from"] == fromRoot.Name
                    && (int) connection["from_port"] == fromPort.PortIndex
                );
        }

        /// <summary>
        /// Checks if he graph edit has a connection originating from the given node on the given port.
        /// </summary>
        public bool HasConnectionTo(TGraphNodeDriver to, Port toPort)
        {
            if (!toPort.IsInput)
            {
                throw new ArgumentException("toPort must be an input port");
            }

            var graphEdit = PresentRoot;
            var toRoot = to.PresentRoot;

            return graphEdit.GetConnectionList().Cast<Dictionary>()
                .Any(connection =>
                    (string) connection["to"] == toRoot.Name
                    && (int) connection["to_port"] == toPort.PortIndex
                );
        }


        public IEnumerable<TGraphNodeDriver> Nodes =>
            BuildDrivers(root => root.GetChildren().OfType<TGraphNode>(),
                node => _nodeDriverProducer(node, "-> GraphNode")
            );
    }

    /// <summary>
    /// Driver for a <see cref="GraphEdit"/>.
    /// </summary>
    [PublicAPI]
    public class GraphEditDriver : GraphEditDriver<GraphEdit, GraphNodeDriver, GraphNode>
    {
        public GraphEditDriver(Func<GraphEdit> producer, string description = "") : base(producer,
            (node, nodeDescription) => new GraphNodeDriver(node, $"{description}-> {nodeDescription}"),
            description)
        {
        }
    }
}