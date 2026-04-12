using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Navigation;
using System.Windows;

namespace MES.Presentation.UI.Controls;

public partial class SideMenuViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

    public SideMenuViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    [ObservableProperty]
    private AppPage activePage = AppPage.Overview;

    [ObservableProperty]
    private bool isExpanded = false;

    public double MenuWidth => IsExpanded ? 200 : 50;

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
        _navigationService.Navigate(page);
    }
}