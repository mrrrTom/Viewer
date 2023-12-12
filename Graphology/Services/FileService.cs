using Graphology.Models.FileStructure;
using System.Collections.Generic;
using System.IO;

namespace Graphology.Services
{
    public class FileService
    {
        private Dictionary<string, FileNode> _fileNodes;
        private static string GetName(string path)
        {
            //ToDo it can be \ and // and \\
            var arr = path.Split('.');
            var partWE = arr[^2];
            var result = partWE.Split('\\').Last();

            return result;
        }

        public void SaveFileContent(string nodeName, string content)
        {
            if (!_fileNodes.ContainsKey(nodeName)) return;
            var filePath = _fileNodes[nodeName].Path;
            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, content);
            }
        }

        public string GetFileContent(string node)
        {
            if (!_fileNodes.ContainsKey(node)) return string.Empty;
            var filePath = _fileNodes[node].Path;
            var fileContent = string.Empty;
            if (File.Exists(filePath))
            {
                fileContent = File.ReadAllText(filePath);
            }

            return fileContent;
        }

        public List<FileNode> GetFiles(string directory)
        {
            _fileNodes = new Dictionary<string, FileNode>();
            var filePaths = Directory.GetFiles(directory).ToHashSet<string>();
            var result = new List<FileNode>();
            var nodeNames = new HashSet<string>();
            foreach (var filePath in filePaths)
            {
                //ToDo is it fast enough?
                var fileContent = File.ReadAllText(filePath);
                var pathEdges = GetEdgesFromContent(directory, fileContent);
                var nameEdges = new List<FileEdge>();
                foreach (var edge in pathEdges)
                {
                    if (!filePaths.Contains(edge.Target))
                    {
                        continue;
                    }

                    nameEdges.Add(new FileEdge(GetName(edge.Target), edge.Title));
                }

                var nodeName = GetName(filePath);
                if (nodeNames.Add(nodeName)) 
                {
                    var node = new FileNode(filePath, nodeName, nameEdges);
                    _fileNodes.Add(nodeName, node);
                    result.Add(node);
                }
            }

            return result;
        }

        private List<FileEdge> GetEdgesFromContent(string dir, string fileContent)
        {
            var collector = new EdgeCollector(dir);
            for (var i = 0; i < fileContent.Length; i++)
            {
                collector.AddSymbol(fileContent[i]);
            }

            return collector.Edges;
            //ToDo user masks
        }
    }

    internal class EdgeCollector
    {
        internal EdgeCollector (string dir)
        {
            _dir = dir;
        }

        private enum CollectorStatus
        {
            TitleClosed,
            TitleOpened,
            TitleReady,
            TitleDone,
            FileOpened  
        }

        public List<FileEdge> Edges { get; private set; } = new List<FileEdge>();
        private string _dir = string.Empty;
        private char _previous;
        private int _fileCount;
        private int _titleCount;
        private CollectorStatus _status;
        //ToDo string -> stringBuilder
        private string _title = string.Empty;
        private string _file = string.Empty;
        
        private void CloseTitle()
        {
            _titleCount = 0;
            _fileCount = 0;
            _title = string.Empty;
            _file = string.Empty;
            _status = CollectorStatus.TitleClosed;
        }

        private void ProcessCharacter(char chr)
        {
            if (_status == CollectorStatus.TitleOpened)
            {
                _titleCount++;

                if (_titleCount > 30 && chr != '-')
                {
                    CloseTitle();
                    return;
                }

                if (_titleCount < 31)
                {
                    _title += chr;
                }
            }

            if (_status == CollectorStatus.FileOpened) 
            {
                _fileCount++;

                if (_fileCount < 1001)
                {
                    _file += chr;
                }
            }

            if (_status == CollectorStatus.TitleDone && chr != '[') 
            {
                CloseTitle();
                return;
            }

            if (_status == CollectorStatus.TitleReady && chr != '>')
            {
                CloseTitle();
                return;
            }

            if (chr == '-' && _previous == '-')
            {
                if (_status == CollectorStatus.TitleOpened)
                {
                    _status = CollectorStatus.TitleReady;
                    if (_title.Length > 1)
                    {
                        if (_title[^1] == '-') _title = _title.Remove(_title.Length -1);
                        if (_title[^1] == '-') _title = _title.Remove(_title.Length -1);
                    }

                    return;
                }

                if (_status == CollectorStatus.TitleClosed)
                {
                    _status = CollectorStatus.TitleOpened;
                    return;
                }

                return;
            }

            if (chr == '>')
            {
                if (_status == CollectorStatus.TitleReady)
                {
                    _status = CollectorStatus.TitleDone;
                }
                else
                {
                    CloseTitle();
                }

                return;
            }

            if (chr == '[')
            {
                if (_previous == '[')
                {
                    if (_status == CollectorStatus.TitleDone)
                    {
                        _status = CollectorStatus.FileOpened;
                    }
                }

                return;
            }

            if (chr == ']')
            {
                if (_status == CollectorStatus.FileOpened)
                {
                    if (_file.Length > 0 && _file[^1] == ']') _file = _file.Remove(_file.Length-1);
                    var newEdge = new FileEdge(GetFilePath(_file), _title);
                    Edges.Add(newEdge);
                    CloseTitle();
                }
            }
        }

        public void AddSymbol(char chr)
        {
            ProcessCharacter(chr);
            _previous = chr;
        }

        private string GetFilePath (string input)
        {
            var result = string.Empty;
            var filePath = input.TrimStart();
            if (!filePath.Contains(_dir))
            {
                result = _dir + @"\" + filePath;
            }
            else
            {
                result = filePath;
            }

            return result;
        }

    }
}