using Graphology.Models;
using Graphology.Models.Drawing;
using Graphology.Models.Graph;
using Graphology.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
using System.Numerics;

namespace Graphology;

public partial class MainPage : ContentPage
{
    private string _directory;
    private Point? _tap;

	public MainPage()
	{
		InitializeComponent();
	}

    void OnPointerEntered(object sender, PointerEventArgs e)
    {
        // Handle the pointer entered event
    }

    void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is GraphicsView gv)
        {
            if (gv.Drawable is GraphView gw)
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        gw.SetPointerStartPosition();
                        gw.TrySetSelected();
                        break;
                    case GestureStatus.Running:
                        if (!gw.IsNodeSelected()) return;
                        gw.MoveSelected();
                        gv.Invalidate();
                        break;
                    case GestureStatus.Completed:
                        if (!gw.IsNodeSelected()) return;
                        gw.SwapWithSelected();
                        gv.Invalidate();
                        break;
                }  
            }
        }
    }

    void OnTapGestureRecognizerTapped(object sender, TappedEventArgs args)
    {
        if (sender is GraphicsView gv)
        {
            var position = args.GetPosition((View)sender);
            if (gv.Drawable is GraphView gw)
            {
                gw.SetPointerPosition(position.Value.X, position.Value.Y);
                if (gw.TryGetFileContent(out var nodeName, out var content))
                {
                    var fe = new FileEditor(nodeName, content, gw);
                    var editorWindow = new Window(fe);
                    Application.Current.OpenWindow(editorWindow);
                }
            }
        }
    }

    void OnEntryTextChanged(object sender, EventArgs e)
    {
        _directory = ((Entry)sender).Text;
    }

    //ToDo async
    void OnRefreshClicked(object sender, EventArgs args)
    {
        if (sender is Button bttn)
        {
            if (bttn.Parent.Parent is Grid grd)
            {
                if (grd.Children.Count > 0 && grd.Children.Last() is GraphicsView gv) 
                {
                    if (gv.Drawable is GraphView gw)
                    {
                        gw.SetFileDirectory(_directory);
                        gv.Invalidate();
                    }
                }
            }
        }
    }

    void OnEntryCompleted(object sender, EventArgs args)
    {
        if (sender is Entry entry)
        {
            if (entry.Parent.Parent is Grid grd)
            {
                if (grd.Children.Count > 0 && grd.Children.Last() is GraphicsView gv)
                {
                    if (gv.Drawable is GraphView gw)
                    {
                        gw.SetFileDirectory(_directory);
                        gv.Invalidate();
                    }
                }
            }
        }
    }

    void OnPointerExited(object sender, PointerEventArgs e)
    {
        // Handle the pointer exited event
    }

    void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (sender is GraphicsView gv)
        {
            var position = e.GetPosition((View)sender);
            if (gv.Drawable is GraphView gw)
            {
                gw.SetPointerPosition(position.Value.X, position.Value.Y);
                gv.Invalidate();
            }
        }
    }
}