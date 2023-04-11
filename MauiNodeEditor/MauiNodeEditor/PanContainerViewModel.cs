using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiNodeEditor.Extensions;
using MauiNodeEditor.Utils;
using Microsoft.Maui.Controls.Shapes;

namespace MauiNodeEditor;

public partial class PanContainerViewModel : ObservableObject, IViewModel
{
    [ObservableProperty]
    private ObservableCollection<Line> edges = new();

    [ObservableProperty]
    private InputHandle startHandle;

    [ObservableProperty]
    private InputHandle arrivalHandle;
    
    [ObservableProperty]
    private Node selectedNode;

    public void SubmitNewEdge()
    {        
        var newEdge = new Line(
            StartHandle.GetPosition(StartHandle.ParentNode.Parent).X + StartHandle.Width / 2,
            StartHandle.GetPosition(StartHandle.ParentNode.Parent).Y + StartHandle.Height / 2,
            ArrivalHandle.GetPosition(StartHandle.ParentNode.Parent).X + ArrivalHandle.Width / 2,
            ArrivalHandle.GetPosition(StartHandle.ParentNode.Parent).Y + ArrivalHandle.Height / 2
        );

        Edges.Add(newEdge);
        ResetEdgeCreation();
    }

    //This must be a responsibility of viewModel because event handling order can't be garanteed.
    private void ResetEdgeCreation()
    {
        StartHandle = null;
        ArrivalHandle = null;
        SelectedNode = null;
    }
}
