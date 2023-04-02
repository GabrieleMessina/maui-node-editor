using MauiNodeEditor.Interfaces;

namespace MauiNodeEditor;

public partial class Node : ContentView, PannableContent
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(Node));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    //TODO: inputhandle should be a list.
    public InputHandle InputHandle => inputHandle;

    public Node()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public void HandlePanUpdate(object sender, PanUpdatedEventArgs e, IPanContainer container)
    {
        HandleNodeMoving(e, container);
    }

    private void HandleNodeMoving(PanUpdatedEventArgs e, IPanContainer container)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                HandlePanTranslation(e, container);
                break;
            case GestureStatus.Started:
            case GestureStatus.Canceled:
            case GestureStatus.Completed:
                SaveLastPanCoords();
                break;
            default:
                break;
        }
    }

    private double currentElementLastTranslationX, currentElementLastTranslationY;

    private void HandlePanTranslation(PanUpdatedEventArgs e, IPanContainer container)
    {
        //TranslationX = Math.Min(Math.Max(0, currentElementLastTranslationX + e.TotalX), container.Width - Width);
        //TranslationY = Math.Min(Math.Max(0, currentElementLastTranslationY + e.TotalY), container.Height - Height);
        TranslationX = currentElementLastTranslationX + e.TotalX;
        TranslationY = currentElementLastTranslationY + e.TotalY;
    }

    private void SaveLastPanCoords()
    {
        currentElementLastTranslationX = TranslationX;
        currentElementLastTranslationY = TranslationY;
    }
}
