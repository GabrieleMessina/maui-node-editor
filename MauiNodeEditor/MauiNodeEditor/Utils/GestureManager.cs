namespace MauiNodeEditor.Utils;
public static partial class GestureManager
{
    public static partial void SubscribeToClickEvent(IElement element, EventHandler<EventArgs.TappedEventArgs> eventHandler);
    public static partial void SubscribeToPointerPressed(IElement element, EventHandler<EventArgs.PointerPressedEventArgs> eventHandler);
    public static partial void SubscribeToPointerReleased(IElement element, EventHandler<EventArgs.PointerPressedEventArgs> eventHandler);

#if !WINDOWS
    public static partial void SubscribeToClickEvent(IElement element, EventHandler<EventArgs.TappedEventArgs> eventHandler)
    {
    }
    public static partial void SubscribeToPointerPressed(IElement element, EventHandler<EventArgs.PointerPressedEventArgs> eventHandler)
    {
    }
    public static partial void SubscribeToPointerReleased(IElement element, EventHandler<EventArgs.PointerPressedEventArgs> eventHandler)
    {
    }
#endif
}
