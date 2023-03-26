namespace MauiNodeEditor.Utils.XamlComponent;

public class ExtendedContentView : ContentView
{
}

public class ExtendedContentView<TViewModel> : ExtendedContentView where TViewModel : IViewModel, new()
{
    public TViewModel ViewModel => (TViewModel)BindingContext;

    protected ExtendedContentView() : base()
    {
        BindingContext = new TViewModel();
    }
}
