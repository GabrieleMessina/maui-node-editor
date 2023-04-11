using SkiaSharp.Views.Maui.Handlers;

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
        //Microsoft.Maui.Handlers.ContentViewHandler.Mapper.AppendToMapping(nameof(ExtendedContentView.PointerWheelChanged), (handler, view) =>
        //{
        //    if (view is ExtendedContentView customView)
        //    {
        //        handler.PlatformView.PointerWheelChanged += (view as ExtendedContentView).PlatformView_PointerWheelChanged;
        //        handler.PlatformView.PointerPressed += PlatformView_PointerPressed;
        //        handler.PlatformView.CanDrag = true;
        //        handler.PlatformView.DragEnter += PlatformView_DragEnter;
        //    }
        //});
#endif
    }
}
