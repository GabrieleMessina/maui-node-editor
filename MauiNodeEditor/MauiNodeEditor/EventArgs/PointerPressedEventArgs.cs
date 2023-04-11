using MauiNodeEditor.Enums;

namespace MauiNodeEditor.EventArgs;
public class PointerPressedEventArgs : BaseEventArgs
{
    public Point PositionRelativeToWindow { get; }
    public Point? PositionRelativeToPage { get; }
    public KeyModifiers KeyModifiers { get; }

    public PointerPressedEventArgs(object platformEventArgs, object platformSender, Point positionRelativeToWindow, Point? positionRelativeToPage, KeyModifiers keyModifiers) : base(platformEventArgs, platformSender)
    {
        PositionRelativeToWindow = positionRelativeToWindow;
        PositionRelativeToPage = positionRelativeToPage;
        KeyModifiers = keyModifiers;
    }
}
