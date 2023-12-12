namespace Graphology.Models.Graph
{
    internal struct GraphEdge
    {
        public readonly Position<float> Start;
        public readonly Position<float> Target;
        public readonly string Title;

        public GraphEdge(Position<float> start, Position<float> target, string title)
        {
            Start = start;
            Target = target;
            Title = title;
        }

        public GraphEdge(float startX, float startY, float targetX, float targetY, string title)
        {
            Start = new Position<float>(startX, startY);
            Target = new Position<float>(targetX, targetY);
            Title = title;
        }
    }
}
