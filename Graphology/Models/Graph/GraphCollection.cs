using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphology.Models.Graph
{
    internal struct GraphCollection
    {
        public readonly List<GraphNode> Nodes;
        public readonly List<GraphEdge> Edges;

        public GraphCollection(List<GraphNode> nodes, List<GraphEdge> edges)
        {
            Nodes = nodes;
            Edges = edges;
        }
    }
}
