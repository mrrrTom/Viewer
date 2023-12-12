using Graphology.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphology.Models.Drawing
{
    internal struct Text
    {
        public readonly Position<float> Position;
        public readonly string Value;
        public readonly float Widht;
        public readonly float Height;
        public readonly HorizontalAlignment HorizontalAlignment;
        public readonly VerticalAlignment VerticalAlignment;
        public readonly Color Color;
        public readonly float Size;
        public readonly IFont Font;

        //ToDo замени интерфейс на стрктуру, чтобы избежать боксинга
        public Text(Position<float> position, string value, float widht, float height, HorizontalAlignment ha, VerticalAlignment va, Color color, float stroke, IFont font)
        {
            Position = position;
            Value = value;
            Widht = widht;
            Height = height;
            HorizontalAlignment = ha;
            VerticalAlignment = va;
            Color = color;
            Size = stroke;
            Font = font;
            var k = 0.ToString();
        }
    }
}
