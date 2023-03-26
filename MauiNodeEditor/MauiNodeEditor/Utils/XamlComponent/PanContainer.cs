namespace MauiNodeEditor.Utils.XamlComponent;

public class PanContainer : ExtendedContentView<PanContainerViewModel>
{
    private double lastPanXTranslation, lastPanYTranslation;

    public PanContainer()
    {
        // Set PanGestureRecognizer.TouchPoints to control the
        // number of touch points needed to pan
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(panGesture);
    }

    private void HandlePanGesture(PanUpdatedEventArgs e)
    {
        Content.TranslationX = Math.Min(Math.Max(0, lastPanXTranslation + e.TotalX), Width - Content.Width);
        Content.TranslationY = Math.Min(Math.Max(0, lastPanYTranslation + e.TotalY), Height - Content.Height);
    }
    
    private void SaveLastPanCoords()
    {
        lastPanXTranslation = Content.TranslationX;
        lastPanYTranslation = Content.TranslationY;
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
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
}
