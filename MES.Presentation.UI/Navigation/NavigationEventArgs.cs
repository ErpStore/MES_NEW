namespace MES.Presentation.UI.Navigation;

public class NavigationEventArgs : EventArgs
{
    public AppPage Page { get; }

    public NavigationEventArgs(AppPage page)
    {
        Page = page;
    }
}