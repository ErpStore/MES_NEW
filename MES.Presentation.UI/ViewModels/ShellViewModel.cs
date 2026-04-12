using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Controls;
using MES.Presentation.UI.Modules.Materials.ViewModel;
using MES.Presentation.UI.Modules.Order.ViewModel;
using MES.Presentation.UI.Modules.OverView.ViewModels;
using MES.Presentation.UI.Modules.Recipe.ViewModel;
using MES.Presentation.UI.Modules.UserManagement.ViewModels;
using MES.Presentation.UI.Navigation;
using MES.Presentation.UI.Service;

namespace MES.Presentation.UI.ViewModels;

public partial class ShellViewModel : BaseViewModel
{
    private readonly IMediator _mediator; // The "Waiter"
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly INavigationService _navigationService;
    private const string DefaultTitle = "TRIKALA ERP";

    [ObservableProperty]
    private HeaderBarViewModel headerBar;

    [ObservableProperty]
    private SideMenuViewModel sideMenu;

    [ObservableProperty]
    private BaseViewModel? currentViewModel;

    public string ApplicationTitle => DefaultTitle;

    public ShellViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory,
        INavigationService navigationService, HeaderBarViewModel headerBar, SideMenuViewModel sideMenu)
    {
        _mediator = mediator;
        _dialogService = dialogService;
        _viewModelFactory = viewModelFactory;
        _navigationService = navigationService;
        _navigationService.NavigateRequested += NavigateTo;
        this.headerBar = headerBar;
        this.sideMenu = sideMenu;
    }

    public async void NavigateTo(object? sender, NavigationEventArgs e)
    {
        CurrentViewModel = e.Page switch
        {
            AppPage.Overview => new OverviewViewModel(),
            AppPage.Users => new UsersViewModel(_mediator, _dialogService, _viewModelFactory),
            AppPage.Material => new MaterialsViewModel(_mediator, _dialogService, _viewModelFactory),
            AppPage.Recipe => new RecipesViewModel(_mediator, _dialogService, _viewModelFactory),
            AppPage.Orders => new OrderManagementViewModel(_viewModelFactory),
            _ => throw new NotSupportedException($"Page {e.Page} not supported")
        };

        await CurrentViewModel.InitializeAsync();
    }
}