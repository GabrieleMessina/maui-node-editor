namespace MauiNodeEditor.Utils.XamlComponent;

public class ExtendedContentView : ContentView
{
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
        BindingContext = new TViewModel();
    }
}
