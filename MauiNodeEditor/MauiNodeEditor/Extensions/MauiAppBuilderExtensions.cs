using MauiNodeEditor.Utils.XamlComponent;
using SkiaSharp.Views.Maui.Handlers;
using Microsoft.Maui.Handlers;
using MauiNodeEditor.Handlers;
using Microsoft.Maui.Hosting;
#if WINDOWS
using Microsoft.UI.Xaml;
#endif
namespace MauiNodeEditor.Extensions;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMauiNodeEditor(this MauiAppBuilder builder)
    {
        builder.Services.AddServices();
        builder.Services.AddViewModels();
        builder.Services.AddPages();
        builder.ConfigureCustomHandler();
        builder.UseNodeEditorDrawable();
        return builder;
    }

    public static MauiAppBuilder UseNodeEditorDrawable(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(h =>
        {
            h.AddHandler<NodeEditorSKDrawable, SKCanvasViewHandler>();
            //h.AddHandler<Node, NodeHandler>();
        });

        return builder;
    }


    private static MauiAppBuilder ConfigureCustomHandler(this MauiAppBuilder builder)
    {
        ConfigureExtendedContentView();
        return builder;
    }

    private static void ConfigureExtendedContentView()
    {
#if WINDOWS
        Microsoft.Maui.Handlers.ContentViewHandler.Mapper.AppendToMapping(nameof(ExtendedContentView.PointerWheelChanged), (handler, view) =>
        {
            if (view is ExtendedContentView customView)
            {
                handler.PlatformView.PointerWheelChanged += (view as ExtendedContentView).PlatformView_PointerWheelChanged;
                handler.PlatformView.PointerPressed += PlatformView_PointerPressed;
                handler.PlatformView.CanDrag = true;
                handler.PlatformView.DragEnter += PlatformView_DragEnter;
            }
        });
#endif
    }
#if WINDOWS
    private static void PlatformView_DragEnter(object sender, Microsoft.UI.Xaml.DragEventArgs e)
    {
        var position = e.GetPosition(sender as UIElement);
    }

    private static void PlatformView_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var pointer = e.GetCurrentPoint(sender as UIElement);
    }
#endif
}
