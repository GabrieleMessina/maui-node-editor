namespace MauiNodeEditor.Utils;

public class ExtendedContentPage : ContentPage
{
}

public class ExtendedContentPage<TViewModel> : ExtendedContentPage where TViewModel : IViewModel
{
    protected TViewModel ViewModel => (TViewModel)BindingContext;

    protected ExtendedContentPage(TViewModel viewModel) : base()
    {
        BindingContext = viewModel;
    }
}
