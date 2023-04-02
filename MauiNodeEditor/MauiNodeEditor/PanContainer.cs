using System.Collections.ObjectModel;
using MauiNodeEditor.Interfaces;
using MauiNodeEditor.Utils.XamlComponent;
using Microsoft.Maui.Controls.Shapes;

namespace MauiNodeEditor;

public class PanContainer : ExtendedContentView<PanContainerViewModel>, IPanContainer
{
    private Node currentSelectedStartingNode;
    private Node currentSelectedArrivalNode;
    private InputHandle currentSelectedStartingInputHandle;
    private InputHandle currentSelectedArrivalInputHandle;
    private Point currentMousePosition = new();
    private GestureStatus currentGestureStatus = GestureStatus.Completed;
    private Line currentCreatingEdge = new(0, 0, 100, 100);
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

        Unfocused += PanContainer_Unfocused;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Handler != null)
        {
            currentCreatingEdge.Stroke = Brush.Red;
            currentCreatingEdge.StrokeThickness = 3;
            currentCreatingEdge.ZIndex = 30000;
            (Content as AbsoluteLayout).Add(currentCreatingEdge);
        }
    }

    private static void OnNodesChanged(BindableObject bindable, object oldValue, object newValue)
    {
    }

    private void PanContainer_Unfocused(object sender, FocusEventArgs e)
    {
        ResetEdgeCreation();
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        currentMousePosition = e.GetPosition(this) ?? currentMousePosition;
    }

    private void ResetEdgeCreation()
    {
        currentSelectedStartingInputHandle = null;
        currentSelectedArrivalInputHandle = null;
    }

    protected override void OnChildAdded(Element child)
    {
        base.OnChildAdded(child);

        var children = child as Layout;
        contentLayout = child as AbsoluteLayout;

        //pointer recognizers needed for selecting the currentSelectedElement
        var pointerRecognizerForNodes = new PointerGestureRecognizer();
        pointerRecognizerForNodes.PointerEntered += PointerEnteredOnNode;
        pointerRecognizerForNodes.PointerExited += PointerExitedOnNode;

        var pointerRecognizerForInputHandlers = new PointerGestureRecognizer();
        pointerRecognizerForInputHandlers.PointerEntered += PointerEnteredOnInputHandle;
        pointerRecognizerForInputHandlers.PointerExited += PointerExitedOnInputHandle;

        foreach (var view in children.OfType<Node>())
        {
            view.GestureRecognizers.Add(pointerRecognizerForNodes);

            view.InputHandle.GestureRecognizers.Add(pointerRecognizerForInputHandlers);

            Nodes.Add(view);
        }
    }

    private void OnContainerPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        currentGestureStatus = e.StatusType;
        if (currentSelectedStartingNode == null) //pan the entire view
        {
            HandleContainerPan(e);
            return;
        }
        else
        {
            if (currentSelectedStartingInputHandle == null) //pan the node
            {
                //TODO: there is the risk that not all the status type are redirect correctly.
                //TODO: sender and this are the probably same.
                (currentSelectedStartingNode as PannableContent).HandlePanUpdate(sender, e, this);
                return;
            }
            else //create an adge
            {
                HandleEdgeDrawing(currentGestureStatus);
            }
        }

    }

    private const double edgeLinePadding = 10d; //this is needed for allow the cursor to actualy going upon input handlers and trigger the gesture recognizer, otherwise the line is alway behind the cursor.
    private void HandleEdgeDrawing(GestureStatus currentGestureStatus)
    {
        if (currentGestureStatus == GestureStatus.Started)
        {
            currentCreatingEdge.X1 = currentSelectedStartingNode.TranslationX + currentSelectedStartingNode.Width - (currentSelectedStartingNode.Width - (currentSelectedStartingInputHandle.X + currentSelectedStartingInputHandle.Width));
            currentCreatingEdge.Y1 = currentSelectedStartingNode.TranslationY + currentSelectedStartingNode.Height - (currentSelectedStartingNode.Height - (currentSelectedStartingInputHandle.Y + currentSelectedStartingInputHandle.Height));
            currentCreatingEdge.IsVisible = true;
        }
        if (currentGestureStatus == GestureStatus.Running)
        {
            currentCreatingEdge.X2 = currentMousePosition.X;
            currentCreatingEdge.Y2 = currentMousePosition.Y;

        }
        if (currentGestureStatus == GestureStatus.Completed)
        {
            if (currentSelectedArrivalInputHandle != null)
            {
                currentCreatingEdge.X2 = currentSelectedArrivalNode.TranslationX + currentSelectedArrivalNode.Width - (currentSelectedArrivalNode.Width - (currentSelectedArrivalInputHandle.X + currentSelectedArrivalInputHandle.Width)) + edgeLinePadding;
                currentCreatingEdge.Y2 = currentSelectedArrivalNode.TranslationY + currentSelectedArrivalNode.Height - (currentSelectedArrivalNode.Height - (currentSelectedArrivalInputHandle.Y + currentSelectedArrivalInputHandle.Height)) + edgeLinePadding;
                ResetEdgeCreation();
            }
            else
            {
                ResetEdgeCreation();
                currentCreatingEdge.IsVisible = false;
            }
        }
        if (currentGestureStatus == GestureStatus.Canceled)
        {
            ResetEdgeCreation();
            currentCreatingEdge.IsVisible = false;
        }
        InvalidateLayout();
    }


    #region Choose CurrentSelectedElement
    private void PointerEnteredOnNode(object sender, PointerEventArgs e)
    {
        var node = sender as Node;
        if (currentSelectedStartingNode != null && currentSelectedStartingNode != node && currentGestureStatus == GestureStatus.Running)
        {
            currentSelectedArrivalNode = node;
        }

        //avoid to change element while moving the current one.
        if (currentGestureStatus != GestureStatus.Running)
        {
            currentSelectedStartingNode = node;
        }
    }

    private void PointerExitedOnNode(object sender, PointerEventArgs e)
    {
        //avoid to change element while moving the current one.
        if (currentGestureStatus != GestureStatus.Running && currentSelectedStartingNode == sender)
        {
            currentSelectedStartingNode = null;
        }
    }

    private void PointerEnteredOnInputHandle(object sender, PointerEventArgs e)
    {
        //avoid to change element while moving the current one.
        var inputHandler = sender as InputHandle;
        if (currentSelectedStartingInputHandle != null && currentSelectedStartingInputHandle != inputHandler && currentGestureStatus == GestureStatus.Running)
        {
            currentSelectedArrivalInputHandle = inputHandler;
        }

        if (currentGestureStatus != GestureStatus.Running)
        {
            currentSelectedStartingInputHandle = inputHandler;
        }
    }

    private void PointerExitedOnInputHandle(object sender, PointerEventArgs e)
    {
        //avoid to change element while moving the current one.
        if (currentGestureStatus != GestureStatus.Running && currentSelectedStartingInputHandle == sender)
        {
            currentSelectedStartingInputHandle = null;
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
