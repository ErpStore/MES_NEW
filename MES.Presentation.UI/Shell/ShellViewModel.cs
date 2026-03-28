
using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Modules.Materials.ViewModel;
using MES.Presentation.UI.Modules.Order.ViewModel;
using MES.Presentation.UI.Modules.OverView.ViewModels;
using MES.Presentation.UI.Modules.Recipe.ViewModel;
using MES.Presentation.UI.Modules.UserManagement.ViewModels;
using MES.Presentation.UI.Navigation;
using MES.Presentation.UI.Service;
using Microsoft.Extensions.Logging;


namespace MES.Presentation.UI.Shell
{
    public partial class ShellViewModel : BaseViewModel
    {
        private readonly IMediator _mediator; // The "Waiter"
        private readonly IDialogService _dialogService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly ILogger<ShellViewModel>? _logger;

        [ObservableProperty]
        private BaseViewModel? currentViewModel;

        public ShellViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory)
        {
            _mediator = mediator;
            _dialogService = dialogService;
            _viewModelFactory = viewModelFactory;

            NavigateTo(AppPage.Recipe);
        }

        public async void NavigateTo(AppPage page)
        {
            CurrentViewModel = page switch
            {
                AppPage.Overview => new OverviewViewModel(),
                AppPage.Users => new UsersViewModel(_mediator, _dialogService, _viewModelFactory),
                AppPage.Material => new MaterialsViewModel(_mediator, _dialogService, _viewModelFactory),
                AppPage.Recipe => new RecipesViewModel(_mediator, _dialogService, _viewModelFactory),
                AppPage.Orders => new OrderManagementViewModel(_viewModelFactory),

                _ => throw new NotSupportedException($"Page {page} not supported")
            };

           await CurrentViewModel.InitializeAsync();
        }
    }
}
