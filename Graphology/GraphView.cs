using Graphology.Models.Drawing;
using Graphology.Models.Graph;
using Graphology.Services;
using static Graphology.Services.GraphService;

namespace Graphology;

public class GraphView : IDrawable
{
    private string _fileDirectory;
    private bool _newDir;
    private double _pointerX;
    private double _pointerStartX;
    private double _pointerY;
    private double _pointerStartY;
    private bool _isNodeSelected;
    private string _selectedNode;
    private float _width;
    private float _height;
    private List<Line> _lines = new List<Line>();
    private List<Circle> _circles = new List<Circle>();
    private List<Text> _texts = new List<Text>();
    private GraphService _graphService;
    private Graph _graph;

    public GraphView()
    {
        _graphService = new GraphService();
    }

    public void SetFileDirectory(string fd)
    {
        _fileDirectory = fd;
        _newDir = true;
    }

    public string GetFileDirectory()
    {
        return _fileDirectory;
    }

    public bool IsNodeSelected()
    { 
        return _isNodeSelected;
    }

    public void SetPointerStartPosition()
    {
        _pointerStartX = _pointerX;
        _pointerStartY = _pointerY;
    }

    public void SetPointerPosition(double x, double y)
    {
        _pointerX = x;
        _pointerY = y;
    }

    internal void SetTexts(List<Text> texts)
    {
        _texts = texts;
    }

    internal void SetLines(List<Line> lines)
    {
        _lines = lines;
    }

    internal void SetCircles(List<Circle> circles)
    {
        _circles = circles;
    }

    public void SaveFileContent(string nodeName, string content)
    {
        _graphService.SaveFileContent(nodeName, content);
    }

    public bool TrySetSelected()
    {
        if (_graph.TryGetPointedNode(_pointerStartX, _pointerStartY, out var nodeName))
        {
            _selectedNode = nodeName;
            _isNodeSelected = true;
            return true;
        }

        return false;
    }

    public void MoveSelected()
    {
        var deltaX = _pointerStartX - _pointerX;
        var deltaY = _pointerStartY - _pointerY;
        _graphService.MoveSelected(_graph, deltaX, deltaY, _selectedNode);
    }

    public void SwapWithSelected()
    {
        _graphService.SwapWithSelected(_graph, _pointerX, _pointerY, _selectedNode);
        _isNodeSelected = false;
        _selectedNode = string.Empty;
    }

    public bool TryGetFileContent(out string nodeName, out string fileContent)
    {
        return _graphService.TryGetFileContent(_graph, _pointerX, _pointerY, out nodeName, out fileContent);
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (string.IsNullOrEmpty(_fileDirectory)) return;
        if (_width != dirtyRect.Width || _height != dirtyRect.Height || _graph == null || _newDir) 
        {
            _width = dirtyRect.Width;
            _height = dirtyRect.Height;
            _graph = _graphService.CreateGraph(_width, _height, _fileDirectory);
            _newDir = false;
        }

        if (_isNodeSelected)
        {
            _graphService.FillView(this, _graph, _pointerStartX, _pointerStartY);
        }
        else
        {
            _graphService.FillView(this, _graph, _pointerX, _pointerY);
        }
        
        foreach (var line in _lines)
        {
            canvas.StrokeSize = line.Stroke;
            canvas.StrokeColor = line.Color;
            canvas.DrawLine(line.Start.X, line.Start.Y, line.End.X, line.End.Y);
        }

        foreach (var circle in _circles)
        {
            canvas.StrokeSize = circle.Stroke;
            canvas.FillColor = Colors.White;
            canvas.FillCircle(circle.Center.X, circle.Center.Y, circle.Radius);
            canvas.StrokeColor = circle.Color;
            canvas.FontColor = circle.Color;
            canvas.DrawCircle(circle.Center.X, circle.Center.Y, circle.Radius);
        }

        foreach(var text in _texts)
        {
            if (string.IsNullOrWhiteSpace(text.Value)) continue;
            canvas.FontSize = text.Size;
            canvas.FontColor = text.Color;
            canvas.Font = text.Font;
            canvas.DrawString(text.Value, text.Position.X, text.Position.Y, text.Widht, text.Height, text.HorizontalAlignment, text.VerticalAlignment);
        }
    }
}