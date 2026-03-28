using CommunityToolkit.Mvvm.ComponentModel;
using MES.Presentation.UI.Shell;
using Microsoft.Extensions.DependencyInjection;

namespace MES.Presentation.UI.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly ShellViewModel _shell;

        public NavigationService(ShellViewModel shell)
        {
            _shell = shell;
        }

        public void Navigate(AppPage page)
        {
            _shell.NavigateTo(page);
        }
    }
}
