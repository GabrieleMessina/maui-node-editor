using MauiNodeEditor.Extensions;
using Microsoft.UI.Xaml;

namespace MauiNodeEditor.EventArgs;

public partial class TappedEventArgs : BaseEventArgs
{
    public partial Point? PositionRelativeTo(IElement element)
    {
        return (platformEventArgs as Microsoft.UI.Xaml.Input.TappedRoutedEventArgs).GetPosition(element.Handler.PlatformView as UIElement).ToShared();
    }
}
