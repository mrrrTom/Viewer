using Graphology.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphology.Models.Drawing
{
    internal struct Circle
    {
        public readonly Position<float> Center;
        public readonly float Radius;
        public readonly Color Color;
        public readonly float Stroke;

        public Circle(float x, float y, float radius, Color color, float stroke)
        {
            Center = new Position<float>(x, y);
            Radius = radius;
            Color = color;
            Stroke = stroke;
        }

        public Circle(Position<float> center, float radius, Color color, float stroke)
        {
            Center = center;
            Radius = radius;
            Color = color;
            Stroke = stroke;
        }
    }
}
