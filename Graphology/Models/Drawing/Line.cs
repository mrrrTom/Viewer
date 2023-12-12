using Graphology.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphology.Models.Drawing
{
    internal struct Line
    {
        public readonly Position<float> Start;
        public readonly Position<float> End;
        public readonly Color Color;
        public readonly float Stroke;

        public Line(float startX, float startY, float endX, float endY, Color color, float stroke)
        {
            Start = new Position<float>(startX, startY);
            End = new Position<float>(endX, endY);
            Color = color;
            Stroke = stroke;
        }

        public Line(Position<float> start, Position<float> end, Color color, float stroke)
        {
            Start = start;
            End = end;
            Color = color;
            Stroke = stroke;
        }
    }
}
