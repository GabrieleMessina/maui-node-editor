using MauiNodeEditor.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Application = Microsoft.Maui.Controls.Application;

namespace MauiNodeEditor.Utils;
public static partial class GestureManager
{
    //TODO: check if element.handler exists, otherwise throw an exception explaning to user that element must be already mounted.
    // or try some lazy initializzation.
    public static partial void SubscribeToClickEvent(IElement element, EventHandler<EventArgs.TappedEventArgs> eventHandler)
    {
        var platformElement = element.Handler.PlatformView as UIElement;

        platformElement.Tapped += (sender, e) => PlatformElement_Tapped(sender, e, element, eventHandler);
    }
    
    public static partial void SubscribeToPointerPressed(IElement element, EventHandler<EventArgs.PointerPressedEventArgs> eventHandler)
    {
        var platformElement = element.Handler.PlatformView as UIElement;

        platformElement.PointerPressed += (sender, e) => PlatformElement_PointerPressed(sender, e, element, eventHandler);
    }
    
    public static partial void SubscribeToPointerReleased(IElement element, EventHandler<EventArgs.PointerPressedEventArgs> eventHandler)
    {
        var platformElement = element.Handler.PlatformView as UIElement;

        platformElement.PointerReleased += (sender, e) => PlatformElement_PointerReleased(sender, e, element, eventHandler);
    }

    private static void PlatformElement_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e, IElement mauiElement, EventHandler<EventArgs.PointerPressedEventArgs> eventHandler)
    {
        var window = (Microsoft.UI.Xaml.Window)Application.Current.Windows[0].Handler.PlatformView;

        var windowPosition = e.GetCurrentPoint(null).Position.ToShared();
        var pagePosition = e.GetCurrentPoint(window.Content).Position.ToShared();
        eventHandler.Invoke(mauiElement, new EventArgs.PointerPressedEventArgs(e, sender, windowPosition, pagePosition, e.KeyModifiers.ToShared()));
    }
    
    private static void PlatformElement_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e, IElement mauiElement, EventHandler<EventArgs.PointerPressedEventArgs> eventHandler)
    {
        var window = (Microsoft.UI.Xaml.Window)Application.Current.Windows[0].Handler.PlatformView;

        var windowPosition = e.GetCurrentPoint(null).Position.ToShared();
        var pagePosition = e.GetCurrentPoint(window.Content).Position.ToShared();
        eventHandler.Invoke(mauiElement, new EventArgs.PointerPressedEventArgs(e, sender, windowPosition, pagePosition, e.KeyModifiers.ToShared()));
    }

    private static void PlatformElement_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e, IElement mauiElement, EventHandler<EventArgs.TappedEventArgs> eventHandler)
    {
        var window = (Microsoft.UI.Xaml.Window)Application.Current.Windows[0].Handler.PlatformView;

        var windowPosition = e.GetPosition(null).ToShared();
        var pagePosition = e.GetPosition(window.Content).ToShared();
        Point? parentPosition = null;
        eventHandler.Invoke(mauiElement, new EventArgs.TappedEventArgs(e, sender, windowPosition, pagePosition, parentPosition));
    }
}
