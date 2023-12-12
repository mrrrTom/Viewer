namespace Graphology.Models.FileStructure
{
    public struct FileEdge
    {
        public readonly string Target;
        public readonly string Title;

        public FileEdge(string target, string title)
        {
            if (string.IsNullOrWhiteSpace(target)) throw new ArgumentNullException("target");
            Target = target;
            Title = title;
        }
    }
}
