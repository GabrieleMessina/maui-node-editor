using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MauiNodeEditor.EventArgs;
using MauiNodeEditor.Interfaces;
using MauiNodeEditor.Utils;
using MauiNodeEditor.Utils.XamlComponent;
using Microsoft.Maui.Controls.Shapes;

namespace MauiNodeEditor;

public class PanContainer : ExtendedContentView<PanContainerViewModel>, IPanContainer
{
    private Point currentMousePosition = new();
    private GestureStatus currentGestureStatus = GestureStatus.Completed;
    private AbsoluteLayout contentLayout = new();

    public static readonly BindableProperty NodesProperty = BindableProperty.Create(nameof(Nodes), typeof(ObservableCollection<Node>), typeof(PanContainer), new ObservableCollection<Node>(), propertyChanged: OnNodesChanged);

    public ObservableCollection<Node> Nodes
    {
        get => (ObservableCollection<Node>)GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    public PanContainer()
    {
        //listen for pan gesture on container, later on redirect this event to the current selected child.
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnContainerPanUpdated;
        GestureRecognizers.Add(panGesture);

        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerMoved += OnPointerMoved;
        GestureRecognizers.Add(pointerGesture);

        ViewModel.Edges.CollectionChanged += Edges_CollectionChanged;
    }

    private void Edges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if(e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var edge in e.OldItems.OfType<Line>())
            {
                contentLayout.Remove(edge);
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var edge in e.NewItems.OfType<Line>())
            {
                edge.Stroke = Brush.Red;
                edge.StrokeThickness = 3;
                edge.ZIndex = 100;
                contentLayout.Add(edge);
            }
        }
        
        if (e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Move)
        {
            foreach (var edge in e.OldItems.OfType<Line>())
            {
                contentLayout.Remove(edge);
            }
            foreach (var edge in e.NewItems.OfType<Line>())
            {
                edge.Stroke = Brush.Red;
                edge.StrokeThickness = 3;
                edge.ZIndex = 100;
                contentLayout.Add(edge);
            }
        }
        
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var edge in ViewModel.Edges)
            {
                contentLayout.Remove(edge);
            }
            foreach (var edge in e.NewItems.OfType<Line>())
            {
                edge.Stroke = Brush.Red;
                edge.StrokeThickness = 3;
                edge.ZIndex = 100;
                contentLayout.Add(edge);
            }
        }
    }

    protected override void OnChildAdded(Element child)
    {
        base.OnChildAdded(child);

        var children = child as Layout;

        foreach (var view in children.OfType<Node>())
        {
            Nodes.Add(view);
        }
    }


    protected override void OnInitialized()
    {
        base.OnInitialized();

        contentLayout = Content as AbsoluteLayout;

        if (Handler != null)
        {
            foreach (var view in Nodes)
            {
                GestureManager.SubscribeToPointerPressed(view, NodePointerPressed);
                GestureManager.SubscribeToPointerReleased(view, NodePointerReleased);
                
                GestureManager.SubscribeToPointerPressed(view.InputHandle, InputHandlePointerPressed);
                GestureManager.SubscribeToPointerReleased(view.InputHandle, InputHandlePointerReleased);
            }
        }
    }

    private static void OnNodesChanged(BindableObject bindable, object oldValue, object newValue)
    {
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        currentMousePosition = e.GetPosition(this) ?? currentMousePosition;
    }

    private void OnContainerPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        currentGestureStatus = e.StatusType;

        if (ViewModel.SelectedNode == null && ViewModel.StartHandle == null) //pan the entire view
        {
            HandleContainerPan(e);
        }
        else
        {
            if (ViewModel.StartHandle != null)//create an adge 
            {
                HandleEdgeDrawing(currentGestureStatus);
            }
            else //pan the node
            {
                //TODO: there is the risk that not all the status type are redirect correctly.
                //TODO: sender and this are the probably same.
                (ViewModel.SelectedNode as PannableContent).HandlePanUpdate(sender, e, this);
            }
        }

        InvalidateLayout();
    }

    private const double edgeLinePadding = 10d; //this is needed for allow the cursor to actualy going upon input handlers and trigger the gesture recognizer, otherwise the line is alway behind the cursor.
    private void HandleEdgeDrawing(GestureStatus currentGestureStatus)
    {
        //if (currentGestureStatus == GestureStatus.Started)
        //{
        //    currentCreatingEdge.X1 = currentCreatingEdge.X2 = currentSelectedStartingNode.TranslationX + currentSelectedStartingNode.Width - (currentSelectedStartingNode.Width - (currentSelectedStartingInputHandle.X + currentSelectedStartingInputHandle.Width));
        //    currentCreatingEdge.Y1 = currentCreatingEdge.Y2 = currentSelectedStartingNode.TranslationY + currentSelectedStartingNode.Height - (currentSelectedStartingNode.Height - (currentSelectedStartingInputHandle.Y + currentSelectedStartingInputHandle.Height));
        //    currentCreatingEdge.IsVisible = true;
        //}
        //if (currentGestureStatus == GestureStatus.Running)
        //{
        //    currentCreatingEdge.X2 = currentMousePosition.X;
        //    currentCreatingEdge.Y2 = currentMousePosition.Y;

        //    var angle = MathF.Atan((float)(currentCreatingEdge.Y2 - currentCreatingEdge.Y1) / (float)(currentCreatingEdge.X2 - currentCreatingEdge.X1));
        //    var sin = MathF.Sin(angle);
        //    var cos = MathF.Cos(angle);

        //    if(sin > 0)
        //    {
        //        currentCreatingEdge.Y2 -= edgeLinePadding;
        //    }
        //    else
        //    {
        //        currentCreatingEdge.Y2 += edgeLinePadding;
        //    }
            
        //    if(cos > 0)
        //    {
        //        currentCreatingEdge.X2 -= edgeLinePadding;
        //    }
        //    else
        //    {
        //        currentCreatingEdge.X2 += edgeLinePadding;
        //    }
        //}
        //if (currentGestureStatus == GestureStatus.Completed)
        //{
        //    if (currentSelectedArrivalInputHandle != null)
        //    {
        //        currentCreatingEdge.X2 = currentSelectedArrivalNode.TranslationX + currentSelectedArrivalNode.Width - (currentSelectedArrivalNode.Width - (currentSelectedArrivalInputHandle.X + currentSelectedArrivalInputHandle.Width));
        //        currentCreatingEdge.Y2 = currentSelectedArrivalNode.TranslationY + currentSelectedArrivalNode.Height - (currentSelectedArrivalNode.Height - (currentSelectedArrivalInputHandle.Y + currentSelectedArrivalInputHandle.Height));
        //    }
        //    else
        //    {
        //        currentCreatingEdge.IsVisible = false;
        //    }
        //}
        //if (currentGestureStatus == GestureStatus.Canceled)
        //{
        //    currentCreatingEdge.IsVisible = false;
        //}
    }


    #region Choose CurrentSelectedElement
    private void NodePointerPressed(object sender, PointerPressedEventArgs e)
    {
        var node = sender as Node;
        ViewModel.SelectedNode = node;
    }

    private void NodePointerReleased(object sender, PointerPressedEventArgs e)
    {
        //avoid to change element while moving the current one.
        ViewModel.SelectedNode = null;
    }


    private void InputHandlePointerPressed(object sender, PointerPressedEventArgs e)
    {
        var inputHandle = sender as InputHandle;
        
        ViewModel.StartHandle = inputHandle;
    }
    
    private void InputHandlePointerReleased(object sender, PointerPressedEventArgs e)
    {
        var inputHandle = sender as InputHandle;

        if (ViewModel.StartHandle != null && ViewModel.StartHandle != inputHandle) 
        {
            ViewModel.ArrivalHandle = inputHandle;
            ViewModel.SubmitNewEdge();
        }
    }

    #endregion Choose CurrentSelectedElement

    #region Container Pan Handling
    private void HandleContainerPan(PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                HandlePanTranslation(e);
                break;
            case GestureStatus.Started:
            case GestureStatus.Canceled:
            case GestureStatus.Completed:
                SaveLastPanCoords();
                break;
            default:
                break;
        }
    }

    private double currentElementLastTranslationX, currentElementLastTranslationY;

    private void HandlePanTranslation(PanUpdatedEventArgs e)
    {
        contentLayout.TranslationX = currentElementLastTranslationX + e.TotalX;
        contentLayout.TranslationY = currentElementLastTranslationY + e.TotalY;
    }

    private void SaveLastPanCoords()
    {
        currentElementLastTranslationX = contentLayout.TranslationX;
        currentElementLastTranslationY = contentLayout.TranslationY;
    }
    #endregion Container Pan Handling

    #region ChildSpawn

    private double maxXOccupied = 0d;
    private double maxYOccupied = 0d;
    private readonly double spawnPadding = 50d;
    private bool alreadySpawned = false;
    private void HandleSpawn(View view)
    {
        view.TranslationX = maxXOccupied;
        view.TranslationY = maxYOccupied;

        if (maxXOccupied < Width - spawnPadding)
        {
            maxXOccupied += view.Width;
        }
        else if (maxYOccupied < Height - spawnPadding)
        {
            maxXOccupied = 0d;
            maxYOccupied += view.Height;
        }
        else
        {
            ResetSpawn();
        }
    }

    private static readonly Random randomGenerator = new();
    private void HandleSpawnRandom(View view)
    {
        view.TranslationX = randomGenerator.NextDouble() * Width;
        view.TranslationY = randomGenerator.NextDouble() * Height;
    }

    private void ResetSpawn()
    {
        maxXOccupied = maxYOccupied = 0d;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (!alreadySpawned && width != -1 && height != -1)
        {
            var children = Content as Layout;
            ResetSpawn();

            foreach (var view in children.OfType<Node>())
            {
                //HandleSpawn(view);
                HandleSpawnRandom(view);
            }

            alreadySpawned = true;
        }
    }
    #endregion ChildSpawn
}
