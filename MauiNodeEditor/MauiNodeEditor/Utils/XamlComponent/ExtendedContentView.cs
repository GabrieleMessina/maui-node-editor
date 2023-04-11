#if WINDOWS
using Microsoft.UI.Xaml;
#endif

namespace MauiNodeEditor.Utils.XamlComponent;

public class ExtendedContentView : ContentView
{
    //public static readonly BindableProperty AreTransportControlsEnabledProperty =
    //        BindableProperty.Create(nameof(AreTransportControlsEnabled), typeof(bool), typeof(Video), true);

    //public bool AreTransportControlsEnabled
    //{
    //    get { return (bool)GetValue(AreTransportControlsEnabledProperty); }
    //    set { SetValue(AreTransportControlsEnabledProperty, value); }
    //}
    protected ExtendedContentView() : base()
    {
        BindingContext = this;
    }

    protected virtual void OnInitialized() { }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        OnInitialized();
    }
}

public class ExtendedContentView<TViewModel> : ExtendedContentView where TViewModel : IViewModel, new()
{
    public TViewModel ViewModel => (TViewModel)BindingContext;

    protected ExtendedContentView() : base()
    {
        //TODO: bind to this
        BindingContext = new TViewModel();
    }
}
