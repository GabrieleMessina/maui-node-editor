namespace MauiNodeEditor.Utils.XamlComponent;

public class ExtendedContentPage : ContentPage
{
}

public class ExtendedContentPage<TViewModel> : ExtendedContentPage where TViewModel : IViewModel, new()
{
    protected TViewModel ViewModel => (TViewModel)BindingContext;

    protected ExtendedContentPage() : base()
    {
        BindingContext = new TViewModel();
    }
}
