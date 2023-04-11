namespace MauiNodeEditor.EventArgs;
public class BaseEventArgs : System.EventArgs
{
    protected readonly object platformEventArgs;
    protected readonly object platformSender;

    public BaseEventArgs(object platformEventArgs, object platformSender)
    {
        this.platformEventArgs = platformEventArgs;
        this.platformSender = platformSender;
    }
}
