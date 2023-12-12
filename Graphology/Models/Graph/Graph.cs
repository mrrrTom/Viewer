using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Graphology.Models.Drawing;

namespace Graphology.Models.Graph
{
    internal class Graph
    {
        private string[,] _matrix;
        private List<(Position<int>, string)>[,] _children;
        private readonly float _radius;
        private Cursor _cursor;
        private Dictionary<string, Position<int>> _elements;
        private int _columnsCount;
        private int _rowsCount;
        private Dictionary<Position<int>, Position<double>> _offset = new Dictionary<Position<int>, Position<double>>();

        private int _totalCount => _rowsCount * _columnsCount;

        public Graph(float width, float height, int nodeCount)
        {
            _elements = new Dictionary<string, Position<int>>();
            var square = width * height;
            var elementSquare = square / nodeCount;
            var doubleSquare = (Math.Sqrt(elementSquare));
            var diametr = (float)doubleSquare;

            var wRemainder = width % diametr;
            if (wRemainder > 0)
            {
                int wCount = (int)(width / diametr) + 1;
                diametr = width / wCount;
            }
            _columnsCount = (int)(width / diametr);

            var rowPrev = (int)(height / diametr);
            var hRemainder = height % diametr;
            if (hRemainder > 0 && _columnsCount * rowPrev < nodeCount)
            {
                int hCount = (int)(height / diametr) + 1;
                diametr = height / hCount;
            }
            _rowsCount = (int)(height / diametr);

            _radius = diametr / 2;
#if DEBUG
            if (_totalCount < nodeCount)
            {
                throw new ArgumentException();
            }
#endif
            _cursor = new Cursor(_rowsCount, _columnsCount);
            _matrix = new string[_rowsCount, _columnsCount];
            _children = new List<(Position<int>, string)>[_rowsCount, _columnsCount];
        }

        public void AddElement(string node, List<(string, string)> children)
        {
#if DEBUG
            if (string.IsNullOrEmpty(node))
            {
                throw new ArgumentNullException("node name");
            }
#endif

            if (_elements.ContainsKey(node))
            {
                return;
            }

            var newElementIndexes = _cursor.GetNextInRow();
            PutElement(node, newElementIndexes);
            var newEdges = new List<(string, string)>();
            if (children != null && children.Count != 0)
            {
                var nodeChildren = default(List<(Position<int>, string)>);
                foreach (var edge in children)
                {
                    if (_elements.TryGetValue(edge.Item1, out var el))
                    {
                        var childrenList = _children[newElementIndexes.X, newElementIndexes.Y];
                        if (childrenList == null)
                        {
                            nodeChildren = new List<(Position<int>, string)>();
                            _children[newElementIndexes.X, newElementIndexes.Y] = nodeChildren;
                        }
                        else
                        {
                            nodeChildren = childrenList;
                        }

                        nodeChildren.Add((el, edge.Item2));
                    }
                    else
                    {
                        newEdges.Add(edge);
                    }
                }
            }

            if (newEdges.Count != 0)
            {
                var childrenIndexes = _cursor.GetNeigbours(newEdges.Count);
                var nodeChildren = default(List<(Position<int>, string)>);
                foreach (var newEdge in newEdges)
                {
                    nodeChildren = new List<(Position<int>, string)>();
                    _children[newElementIndexes.X, newElementIndexes.Y] = nodeChildren;
                    var indexes = childrenIndexes.Dequeue();
                    nodeChildren.Add((indexes, newEdge.Item2));
                    PutElement(newEdge.Item1, indexes);
                }
            }
        }

        private void PutElement(string node, Position<int> newElementIndexes)
        {
            _matrix[newElementIndexes.X, newElementIndexes.Y] = node;
            _elements.Add(node, newElementIndexes);
        }

        public void SwapNodes(string node, double endX, double endY)
        {
            if (string.IsNullOrWhiteSpace(node))
            {
#if DEBUG
                throw new ArgumentNullException("node name");
#endif
                return;
            }
            _offset = new Dictionary<Position<int>, Position<double>>();
            var fnCoordinates = _elements[node];
            var endXIndex = 0;
            if (!TryGetIndex(endX, out endXIndex, _columnsCount))
            {
                return;
            }

            var endYIndex = 0;
            if (!TryGetIndex(endY, out endYIndex, _rowsCount))
            {
                return;
            }

            var snCoordinates = new Position<int> (endYIndex, endXIndex);
            var sNode = _matrix[snCoordinates.X, snCoordinates.Y];
            _elements[node] = snCoordinates;
            _elements[sNode] = fnCoordinates;
            _matrix[fnCoordinates.X, fnCoordinates.Y] = sNode;
            _matrix[snCoordinates.X, snCoordinates.Y] = node;
            var fChildren = _children[fnCoordinates.X, fnCoordinates.Y];
            var fChildrenList = new List<(Position<int>, string)>();
            foreach ( var child in fChildren ?? Enumerable.Empty<(Position<int>,string)>())
            {
                fChildrenList.Add((child.Item1, child.Item2));
            }

            var sChildren = _children[snCoordinates.X, snCoordinates.Y];
            var sChildrenList = new List<(Position<int>, string)>();
            foreach (var child in sChildren ?? Enumerable.Empty<(Position<int>, string)>())
            {
                sChildrenList.Add((child.Item1, child.Item2));
            }

            _children[fnCoordinates.X, fnCoordinates.Y] = sChildrenList;
            _children[snCoordinates.X, snCoordinates.Y] = fChildrenList;
            for (var i = 0; i < _children.GetLength(0); i++ )
            {
                for (var j = 0; j < _children.GetLength(1); j++ )
                {
                    var childList = _children[i, j];
                    var newChildList = new List<(Position<int>, string)>();
                    foreach (var child in childList ?? Enumerable.Empty<(Position<int>, string)>())
                    {
                        if ((child.Item1.X == fnCoordinates.X) && (child.Item1.Y == fnCoordinates.Y))
                        {
                            newChildList.Add((snCoordinates, child.Item2));
                        }
                        else if (((child.Item1.X == snCoordinates.X) && (child.Item1.Y == snCoordinates.Y)))
                        {
                            newChildList.Add((fnCoordinates, child.Item2));
                        }
                        else
                        {
                            newChildList.Add(child);
                        }
                    }

                    _children[i, j] = newChildList;
                }
            }
        }

        public void AddNodeOffset(string node, double offsetX, double offsetY)
        {
            if (string.IsNullOrEmpty(node))
            {
                return;
#if DEBUG
                throw new ArgumentNullException("node name");
#endif
            }
            _offset = new Dictionary<Position<int>, Position<double>>();
            _offset.Add(_elements[node], new Position<double> (offsetX, offsetY));
        }

        public List<GraphNode> GetAllNodes()
        {
            var result = new List<GraphNode>();
            for (var row = 0; row < _matrix.GetLength(0); row++)
            {
                for (var col = 0; col < _matrix.GetLength(1); col++)
                {
                    var node = _matrix[row, col];
                    if (!string.IsNullOrWhiteSpace(node))
                    {
                        var coord = GetCoordinates(row, col);
                        result.Add(new GraphNode(node, coord.X, coord.Y));
                    }
                }
            }

            return result;
        }

        public List<GraphEdge> GetAllEdges()
        {
            var result = new List<GraphEdge>();
            for (var row = 0; row < _matrix.GetLength(0); row++)
            {
                for (var col = 0; col < _matrix.GetLength(1); col++)
                {
                    var children = _children[row, col];
                    if (children != null)
                    {
                        var coord = GetCoordinates(row, col);
                        foreach (var child in children)
                        {
                            var childCoord = GetCoordinates(child.Item1.X, child.Item1.Y);
                            result.Add(new GraphEdge(coord.X, coord.Y, childCoord.X, childCoord.Y, child.Item2));
                        }
                    }
                }
            }

            return result;
        }

        public bool TryGetPointedNode(double x, double y, out string nodeName)
        {
            nodeName = string.Empty;
            var xIndex = 0;
            if (!TryGetIndex(x, out xIndex, _columnsCount - 1))
            {
                return false;
            }

            var yIndex = 0;
            if (!TryGetIndex(y, out yIndex, _rowsCount - 1))
            {
                return false;
            }

            nodeName = _matrix[yIndex, xIndex];
            if (string.IsNullOrWhiteSpace(nodeName))
            {
                return false;
            }

            return true;
        }

        public bool TryGetPointedNodeWithChildren(double x, double y, out GraphCollection gc)
        {
            gc = default(GraphCollection);
            var xIndex = 0;
            if (!TryGetIndex(x, out xIndex, _columnsCount - 1))
            {
                return false;
            }

            var yIndex = 0;
            if (!TryGetIndex(y, out yIndex, _rowsCount - 1))
            {
                return false;
            }

            if (TryCollectNodeWithChildren(yIndex, xIndex, out gc)) return true;
            return false;        
        }

        public float GetCellRadius()
        {
            return _radius;
        }

        private bool TryGetIndex(double input, out int index, int max)
        {
            var cellDiameter = _radius * 2;
            index = (int)(input / cellDiameter);
            if (index > max) return false;
            var remainder = (double)input % (double)cellDiameter;
            if (remainder < _radius / 2 || remainder >= (_radius / 2) * 3)
            {
                return false;
            }

            return true;
        }

        private bool TryCollectNodeWithChildren(int row, int col, out GraphCollection collection)
        {
            collection = default(GraphCollection);
            var coords = GetCoordinates(row, col);
            var edges = new List<GraphEdge>();
            var node = _matrix[row, col];
            if (string.IsNullOrWhiteSpace(node))
            {
                return false;
            }

            var nodes = new List<GraphNode> { new GraphNode(_matrix[row, col], coords) };
            var children = _children[row, col];
            if (children != null && children.Count != 0) 
            {
                foreach (var child in children)
                {
                    var endCoords = GetCoordinates(child.Item1.X, child.Item1.Y);
                    var edge = new GraphEdge(coords, endCoords, child.Item2);
                    nodes.Add(new GraphNode(_matrix[child.Item1.X, child.Item1.Y], endCoords));
                    edges.Add(edge);
                }
            }

            collection = new GraphCollection(nodes, edges);
            return true;
        }

        private Position<float> GetCoordinates(int row, int col)
        {
            //row -> y; col -> x;
            var x = col * _radius * 2 + _radius;
            var y = row * _radius * 2 + _radius;
            var indexes = new Position<int>(row, col);
            if (_offset.ContainsKey(indexes))
            {
                x = (float)(x - _offset[indexes].X);
                y = (float)(y - _offset[indexes].Y);
            }

            return new Position<float> (x, y);
        }
    }
}