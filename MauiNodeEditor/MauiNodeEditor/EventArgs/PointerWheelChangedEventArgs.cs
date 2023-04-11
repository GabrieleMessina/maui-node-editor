using MauiNodeEditor.Enums;

namespace MauiNodeEditor.EventArgs;

public class PointerWheelChangedEventArgs : System.EventArgs
{
    public GestureStatus StatusType { get; internal init; }
    public PointerWheelDirection ScrollDirection { get; internal init; } = PointerWheelDirection.Vertical;
    public Point PointerPosition { get; internal init; }
    public double MouseWheelDelta { get; internal init; }
    public KeyModifiers KeyModifiers { get; internal init; }

}
