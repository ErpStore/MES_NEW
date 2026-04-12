namespace MES.Presentation.UI.Navigation;

public interface INavigationService
{
    public event EventHandler<NavigationEventArgs> NavigateRequested;
    void Navigate(AppPage page);
}