using MauiNodeEditor.Enums;

namespace MauiNodeEditor.Extensions;

public static class WindowsToSharedCastExtensions
{
#if WINDOWS
    public static KeyModifiers ToShared(this Windows.System.VirtualKeyModifiers windowsKeyModifiers)
    {
        return windowsKeyModifiers switch
        {
            Windows.System.VirtualKeyModifiers.None => KeyModifiers.None,
            Windows.System.VirtualKeyModifiers.Control => KeyModifiers.Control,
            Windows.System.VirtualKeyModifiers.Menu => KeyModifiers.Menu,
            Windows.System.VirtualKeyModifiers.Shift => KeyModifiers.Shift,
            Windows.System.VirtualKeyModifiers.Windows => KeyModifiers.WindowsOrCmd,
            _ => KeyModifiers.None,
        };
    }
    
    public static Point ToShared(this Windows.Foundation.Point point)
    {
        return new Point(point.X, point.Y);
    }
#endif
}
