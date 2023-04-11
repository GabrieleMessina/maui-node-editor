namespace MauiNodeEditor.Extensions;
public static class ElementExtensions
{
    public static Point GetPosition(this IElement element, IElement relativeTo = null)
    {
        var position = new Point();

#if WINDOWS
        var platformElement = element.Handler.PlatformView as Microsoft.UI.Xaml.UIElement;
        var platformParent = (relativeTo?.Handler?.PlatformView as Microsoft.UI.Xaml.UIElement) ?? null;
        position = platformElement.TransformToVisual(platformParent).TransformPoint(new Windows.Foundation.Point(0,0)).ToShared();
#endif

        return position;
    }
}
