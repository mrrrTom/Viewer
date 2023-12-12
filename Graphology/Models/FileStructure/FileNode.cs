using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphology.Models.FileStructure
{
    public struct FileNode
    {
        public readonly string Path;
        public readonly string Name;
        public readonly List<FileEdge> Edges;

        public FileNode(string path, string name, List<FileEdge> edges)
        {
            Name = name;
            Path = path;
            Edges = edges;
        }
    }
}
