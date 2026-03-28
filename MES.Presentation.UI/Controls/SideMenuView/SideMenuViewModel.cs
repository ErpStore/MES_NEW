using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Navigation;
using System.Windows;

namespace MES.Presentation.UI.Controls
{
    public partial class SideMenuViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;

        public SideMenuViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        [ObservableProperty]
        private AppPage activePage = AppPage.Overview;

        [ObservableProperty]
        private bool isExpanded = false;

        public double MenuWidth => IsExpanded ? 260 : 200;

        partial void OnIsExpandedChanged(bool value)
        {
            OnPropertyChanged(nameof(MenuWidth));
        }

        [RelayCommand]
        private void ToggleMenu()
        {
            IsExpanded = !IsExpanded;
        }

        [RelayCommand]
        private void Exit()
        {
            Application.Current.Shutdown();
        }

        [RelayCommand]
        private void Navigate(AppPage page)
        {
            ActivePage = page;
            _navigation.Navigate(page);
        }
    }
}
