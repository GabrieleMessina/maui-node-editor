namespace MauiNodeEditor.Utils.XamlComponent;

public class PanContainer : ExtendedContentView<PanContainerViewModel>
{
    private double currentElementLastTranslationX, currentElementLastTranslationY;
    private View currentSelectedElement;
    private GestureStatus currentGestureStatus = GestureStatus.Completed;

    public PanContainer()
    {
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(panGesture);
    }

    protected override void OnChildAdded(Element child)
    {
        base.OnChildAdded(child);

        var children = child as Layout;

        var pointerRecognizer = new PointerGestureRecognizer();
        pointerRecognizer.PointerEntered += PointerGestureRecognizer_PointerEntered;
        pointerRecognizer.PointerExited += PointerGestureRecognizer_PointerExited;

        foreach (var view in children.Cast<View>())
        {
            view.GestureRecognizers.Add(pointerRecognizer);
        }
    }

    private double maxXOccupied = 0d;
    private double maxYOccupied = 0d;
    private double spawnPadding = 50d;
    private bool alreadySpawned = false;
    private void HandleSpawn(View view)
    {
        view.TranslationX = maxXOccupied;
        view.TranslationY = maxYOccupied;

        if (maxXOccupied < this.Width - spawnPadding)
        {
            maxXOccupied += view.Width;
        }
        else if (maxYOccupied < this.Height - spawnPadding)
        {
            maxXOccupied = 0d;
            maxYOccupied += view.Height;
        }
        else
        {
            ResetSpawn();
        }
    }

    private static readonly Random randomGenerator = new ();
    private void HandleSpawnRandom(View view)
    {
        view.TranslationX = randomGenerator.NextDouble() * this.Width;
        view.TranslationY = randomGenerator.NextDouble() * this.Height;
    }

    private void ResetSpawn()
    {
        maxXOccupied = maxYOccupied = 0d;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if(!alreadySpawned && width != -1 && height != -1)
        {
            var children = Content as Layout;
            ResetSpawn();

            foreach (var view in children.Cast<View>())
            {
                //HandleSpawn(view);
                HandleSpawnRandom(view);
            }

            alreadySpawned = true;
        }
    }


    private void HandlePanGesture(PanUpdatedEventArgs e)
    {
        if(currentSelectedElement != null) { 
            currentSelectedElement.TranslationX = Math.Min(Math.Max(0, currentElementLastTranslationX + e.TotalX), Width - currentSelectedElement.Width);
            currentSelectedElement.TranslationY = Math.Min(Math.Max(0, currentElementLastTranslationY + e.TotalY), Height - currentSelectedElement.Height);
        }
    }
    
    private void SaveLastPanCoords()
    {
        if(currentSelectedElement != null)
        {
            currentElementLastTranslationX = currentSelectedElement.TranslationX;
            currentElementLastTranslationY = currentSelectedElement.TranslationY;
        }
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        currentGestureStatus = e.StatusType;
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                HandlePanGesture(e);
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

    private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
    {
        //avoid to change element while moving the current one.
        if(currentGestureStatus != GestureStatus.Running)
        {
            currentSelectedElement = sender as View;
        }
    }

    private void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
    {
        //avoid to change element while moving the current one.
        if (currentGestureStatus!= GestureStatus.Running && currentSelectedElement == sender)
        {
            currentSelectedElement = null;
        }
    }
}
