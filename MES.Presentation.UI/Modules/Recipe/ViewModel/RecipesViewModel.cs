using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Controls.ListHeaderBar;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Service;

namespace MES.Presentation.UI.Modules.Recipe.ViewModel;

public enum RecipesTab { RecipeManagement, RecipeProcess }

    public partial class RecipesViewModel : BaseViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly ICurrentUserService? _currentUserService;

    public ListHeaderBarViewModel<RecipesTab>? Header { get; set; }

    [ObservableProperty]
    private BaseViewModel? _currentContentViewModel;

        public RecipesViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory,
            ICurrentUserService? currentUserService = null)
        {
            _viewModelFactory = viewModelFactory;
            _currentUserService = currentUserService;
        }

        public override async Task InitializeAsync()
        {
            var rights = _currentUserService?.GetRights(ScreenKeys.RecipeManagement);

            // Configure the Top Toolbar
            Header = new ListHeaderBarViewModel<RecipesTab>
            {
                CanAdd = rights?.CanAdd ?? true,
                CanEdit = rights?.CanEdit ?? true,
                CanDelete = rights?.CanDelete ?? true,
                CanRefresh = true,

                AddCommand = new RelayCommand(() => SendAction("Add")),
                EditCommand = new RelayCommand(() => SendAction("Edit")),
                DeleteCommand = new RelayCommand(() => SendAction("Delete")),
                RefreshCommand = new RelayCommand(() => SendAction("Refresh"))
            };

            Header.Tabs.Add(RecipesTab.RecipeManagement);
            Header.Tabs.Add(RecipesTab.RecipeProcess);

        Header.TabChangedRequested += OnTabChanged;

            Header.SelectedTab = RecipesTab.RecipeManagement;
            await LoadTabContent(RecipesTab.RecipeManagement);
        }

        private async void OnTabChanged(RecipesTab tab)
        {
            if (Header != null && _currentUserService != null)
            {
                var screenKey = tab == RecipesTab.RecipeManagement
                    ? ScreenKeys.RecipeManagement
                    : ScreenKeys.RecipeProcess;
                var rights = _currentUserService.GetRights(screenKey);
                Header.CanAdd = rights?.CanAdd ?? true;
                Header.CanEdit = rights?.CanEdit ?? true;
                Header.CanDelete = rights?.CanDelete ?? true;
            }

            await LoadTabContent(tab);
        }

        private async Task LoadTabContent(RecipesTab tab)
        {
            CurrentContentViewModel?.Cleanup();

        BaseViewModel newVm = null;

        switch (tab)
        {
            case RecipesTab.RecipeManagement:
                newVm = _viewModelFactory.Create<RecipeListViewModel>();
                break;

            case RecipesTab.RecipeProcess:
                newVm = _viewModelFactory.Create<RecipeProcessListViewModel>();
                break;
        }

        if (newVm != null)
        {
            CurrentContentViewModel = newVm;
            await newVm.InitializeAsync();
        }
    }

    private void SendAction(string action) =>
        WeakReferenceMessenger.Default.Send(new HeaderActionMessage(action));
}