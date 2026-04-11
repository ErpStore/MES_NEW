
using CommunityToolkit.Mvvm.ComponentModel;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Modules.Materials.ViewModel;
using MES.Presentation.UI.Modules.Order.ViewModel;
using MES.Presentation.UI.Modules.OverView.ViewModels;
using MES.Presentation.UI.Modules.Recipe.ViewModel;
using MES.Presentation.UI.Modules.UserManagement.ViewModels;
using MES.Presentation.UI.Navigation;
using MES.Presentation.UI.Service;


namespace MES.Presentation.UI.Shell
{
    public partial class ShellViewModel : BaseViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;

        [ObservableProperty]
        private BaseViewModel? currentViewModel;

        public ShellViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;

            NavigateTo(AppPage.Recipe);
        }

        public async void NavigateTo(AppPage page)
        {
            CurrentViewModel = page switch
            {
                AppPage.Overview => new OverviewViewModel(),
                AppPage.Users => _viewModelFactory.Create<UsersViewModel>(),
                AppPage.Material => _viewModelFactory.Create<MaterialsViewModel>(),
                AppPage.Recipe => _viewModelFactory.Create<RecipesViewModel>(),
                AppPage.Orders => _viewModelFactory.Create<OrderManagementViewModel>(),

                _ => throw new NotSupportedException($"Page {page} not supported")
            };

           await CurrentViewModel.InitializeAsync();
        }
    }
}
