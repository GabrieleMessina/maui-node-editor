namespace MauiNodeEditor.Utils;

public class ExtendedContentView : ContentView
{
}

public class ExtendedContentView<TViewModel> : ExtendedContentView where TViewModel : IViewModel
{
    public TViewModel ViewModel => (TViewModel)BindingContext;
}
