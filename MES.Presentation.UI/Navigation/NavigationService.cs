namespace MES.Presentation.UI.Navigation;

public class NavigationService : INavigationService
{
    public event EventHandler<NavigationEventArgs>? NavigateRequested;

    public void Navigate(AppPage page)
    {
        NavigateRequested?.Invoke(this, new NavigationEventArgs(page));
    }
}