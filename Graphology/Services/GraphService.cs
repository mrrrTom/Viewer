using Graphology.Models.Drawing;
using Graphology.Models.FileStructure;
using Graphology.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Font = Microsoft.Maui.Graphics.Font;

namespace Graphology.Services
{
    internal class GraphService
    {
        private FileService _fileService;

        public GraphService()
        {
            _fileService = new FileService();
        }

        public Graph CreateGraph(float width, float height, string fileDirectory)
        {
            var fileNodes = _fileService.GetFiles(fileDirectory);
            var graph = new Graph(width, height, fileNodes.Count);
            foreach (var node in fileNodes)
            {
                graph.AddElement(node.Name, node.Edges.Select(e => (e.Target, e.Title)).ToList());
            }
            
            return graph;
        }

        private Position<float> Indent(float startX, float startY, float endX, float endY, float indent) 
        {
            var R = Math.Sqrt(Math.Pow(endX - startX, 2) + Math.Pow(endY - startY, 2));
            var otn = R / (R - indent);
            var xSmeh = Math.Abs((endX - startX) / otn);
            var ySmeh = Math.Abs((endY - startY) / otn);
            var resultEndX = default(float);
            if (endX > startX)
            {
                resultEndX = (float)(startX + xSmeh);
            }
            else
            {
                resultEndX = (float)(startX - xSmeh);
            }

            var resultEndY = default(float);
            if (endY < startY)
            {
                resultEndY = (float)(startY - ySmeh);
            }
            else
            {
                resultEndY = (float)(startY + ySmeh);
            }

            return new Position<float> (resultEndX, resultEndY);
        }

        private float CalculateLineLenght(float startX, float startY, float endX, float endY)
        {
            var lineLenght = (float)Math.Sqrt(Math.Pow(endX - startX, 2) + Math.Pow(endY - startY, 2));
            return lineLenght;
        }

        private Position<float> CalculateLineTextPosition(float lineLenght, float startX, float startY, float endX, float endY)
        {
            var halfLine = Indent(startX, startY, endX, endY, lineLenght / 2);
            //Position above center of the line (but in this case you need to rotate string - how?)
            //var xShift = halfLine.X - startX;
            //var deltaY = (8 * xShift) / lineLenght;
            //var yShift = halfLine.Y - startY;
            //var minorXShift = (xShift * (yShift - deltaY)) / yShift;
            //var deltaX = (2 * lineLenght) / (yShift - deltaY) - ((4 * minorXShift) / (float)Math.Pow((yShift - deltaY), 2));
            //var posX = startX + minorXShift + deltaX;
            //var posY = startY + (yShift - deltaY);
            var textPosition = new Position<float>(halfLine.X - lineLenght / 2, halfLine.Y);
            return textPosition;
        }

        public void SaveFileContent(string nodeName, string content)
        {
            _fileService.SaveFileContent(nodeName, content);
        }

        public bool TryGetFileContent(Graph graph, double x, double y, out string nodeName, out string fileContent)
        {
            fileContent = string.Empty;
            nodeName = string.Empty;
            if (graph!= null && graph.TryGetPointedNode(x, y, out nodeName))
            {
                fileContent = _fileService.GetFileContent(nodeName);
                return true;
            }

            return false;
        }

        public void MoveSelected(Graph graph, double deltaX, double deltaY, string nodeName) 
        {
            graph.AddNodeOffset(nodeName, deltaX, deltaY);
        }

        public void SwapWithSelected(Graph graph, double endX, double endY, string nodeName) 
        {
            graph.SwapNodes(nodeName, endX, endY);
        }

        public void FillView(GraphView view, Graph graph, double pointerX, double pointerY)
        {
            var lines = new List<Line>();
            var circles = new List<Circle>();
            var texts = new List<Text>();
            var pointedOnNode = graph.TryGetPointedNodeWithChildren(pointerX, pointerY, out var selectedGraph);
            var radius = graph.GetCellRadius() / 2;
            var allGraphColor = pointedOnNode ? Colors.LightGrey : Colors.Black;
            foreach (var edge in graph.GetAllEdges())
            {
                //var lineLenght = CalculateLineLenght(edge.Start.X, edge.Start.Y, edge.Target.X, edge.Target.Y);
                //var textPosition = CalculateLineTextPosition(lineLenght, edge.Start.X, edge.Start.Y,edge.Target.X,edge.Target.Y);
                //var text = new Text(textPosition, edge.Title, lineLenght - 4, 4, HorizontalAlignment.Center, VerticalAlignment.Bottom);
                //texts.Add(text);
                var line = new Line(edge.Start.X, edge.Start.Y, edge.Target.X, edge.Target.Y, allGraphColor, 1);
                lines.Add(line);
            }

            foreach (var node in graph.GetAllNodes())
            {
                var text = new Text(new Position<float>(node.Position.X - radius + radius / 5, node.Position.Y - radius / 2), node.Name, radius * 2, radius * 2, HorizontalAlignment.Left, VerticalAlignment.Top, allGraphColor, 10, Font.Default);
                texts.Add(text);
                var circle = new Circle(node.Position.X, node.Position.Y, radius, allGraphColor, 1);
                circles.Add(circle);
            }

            if (pointedOnNode)
            {
                var redLines = selectedGraph.Edges;
                foreach (var edge in redLines)
                {
                    var circleIntersect = Indent(edge.Start.X, edge.Start.Y, edge.Target.X, edge.Target.Y, radius);
                    var lineLenght = CalculateLineLenght(edge.Start.X, edge.Start.Y, circleIntersect.X, circleIntersect.Y);
                    var textPosition = CalculateLineTextPosition(lineLenght, edge.Start.X, edge.Start.Y, circleIntersect.X, circleIntersect.Y);
                    var text = new Text(textPosition, edge.Title, lineLenght, 15, HorizontalAlignment.Center, VerticalAlignment.Bottom, Colors.Black, 10, Font.DefaultBold);
                    texts.Add(text);
                    var line = new Line(edge.Start.X, edge.Start.Y, circleIntersect.X, circleIntersect.Y, Colors.Black, 3);
                    lines.Add(line);

                    var tip = Indent(edge.Start.X, edge.Start.Y, circleIntersect.X, circleIntersect.Y, 10);
                    var R2 = Math.Sqrt(Math.Pow(circleIntersect.X - edge.Start.X, 2) + Math.Pow(circleIntersect.Y - edge.Start.Y, 2));
                    var otn3 = R2 / 5;
                    var xSmeh3 = (circleIntersect.Y - edge.Start.Y) / otn3;
                    var ySmeh3 = (circleIntersect.X - edge.Start.X) / otn3;

                    var cX = (float)(tip.X + xSmeh3);
                    var cY = (float)(tip.Y - ySmeh3);
                    var firstArrowHalfTip = new Line(cX, cY, circleIntersect.X, circleIntersect.Y, Colors.Black, 3);
                    lines.Add(firstArrowHalfTip);

                    var dX = (float)(tip.X - xSmeh3);
                    var dY = (float)(tip.Y + ySmeh3);
                    var secondArrowHalfTip = new Line(dX, dY, circleIntersect.X, circleIntersect.Y, Colors.Black, 3);
                    lines.Add(secondArrowHalfTip);
                }

                var redCircles = selectedGraph.Nodes;
                foreach (var node in redCircles)
                {
                
                    var text = new Text(new Position<float>(node.Position.X - radius + radius / 5, node.Position.Y - radius / 2), node.Name, radius * 2, radius * 2, HorizontalAlignment.Left, VerticalAlignment.Top, Colors.Black, 10, Font.DefaultBold);
                    texts.Add(text);
                    var circle = new Circle(node.Position.X, node.Position.Y, radius, Colors.Black, 3);
                    circles.Add(circle);
                }

            }

            view.SetCircles(circles);
            view.SetLines(lines);
            view.SetTexts(texts);
        }
    }
}
