using System.IO;

namespace Graphology;

public partial class FileEditor : ContentPage
{
    private string _nodeName;
    private GraphView _gw;
	public FileEditor(string nodeName, string content, GraphView gw)
	{
        InitializeComponent();
        _nodeName = nodeName;
        _gw = gw;
        editor.Text = content;
    }

    void OnSaveClicked(object sender, EventArgs args)
    {
        if (sender is Button bttn)
        {
            if (bttn.Parent is Grid grd)
            {
                if (grd.Children.Last() is Editor editor)
                {
                    var text = editor.Text;
                    _gw.SaveFileContent(_nodeName, text);
                }
            }
        }
    }
}