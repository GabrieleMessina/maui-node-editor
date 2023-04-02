namespace MauiNodeEditor.Interfaces;

public interface PannableContent
{
    public void HandlePanUpdate(object sender, PanUpdatedEventArgs e, IPanContainer container);
}
