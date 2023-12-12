using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphology.Models.Graph
{
    internal struct GraphNode
    {
        public readonly Position<float> Position;
        public readonly string Name;

        public GraphNode(string name, Position<float> position)
        {
            Name = name;
            Position = position;
        }

        public GraphNode(string name, float positionX, float positionY)
        {
            Name = name;
            Position = new Position<float> (positionX, positionY);
        }
    }
}
