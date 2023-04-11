namespace MauiNodeEditor.EventArgs;

public partial class TappedEventArgs : BaseEventArgs
{
    public Point PositionRelativeToWindow { get; }
    public Point? PositionRelativeToPage { get; }
    public Point? PositionRelativeToParent { get; }

    public TappedEventArgs(object platformEventArgs, object platformSender, Point positionRelativeToWindow, Point? positionRelativeToPage, Point? positionRelativeToParent) : base(platformEventArgs, platformSender)
    {
        PositionRelativeToWindow = positionRelativeToWindow;
        PositionRelativeToPage = positionRelativeToPage;
        PositionRelativeToParent = positionRelativeToParent;
    }

    public partial Point? PositionRelativeTo(IElement element);

#if !WINDOWS
    public partial Point? PositionRelativeTo(IElement element)
    {
        return new();
    }
#endif
}
