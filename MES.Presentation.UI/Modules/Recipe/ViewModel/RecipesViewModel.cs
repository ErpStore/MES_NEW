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

    public ListHeaderBarViewModel<RecipesTab>? Header { get; set; }

    [ObservableProperty]
    private BaseViewModel? _currentContentViewModel;

    public RecipesViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public override async Task InitializeAsync()
    {
        // Configure the Top Toolbar
        Header = new ListHeaderBarViewModel<RecipesTab>
        {
            CanAdd = true,
            CanEdit = true,
            CanDelete = true,
            CanRefresh = true,

            // Broadcast Actions to whichever child tab is currently active
            AddCommand = new RelayCommand(() => SendAction("Add")),
            EditCommand = new RelayCommand(() => SendAction("Edit")),
            DeleteCommand = new RelayCommand(() => SendAction("Delete")),
            RefreshCommand = new RelayCommand(() => SendAction("Refresh"))
        };

        // Add the Tabs based on your screenshots
        Header.Tabs.Add(RecipesTab.RecipeManagement); // Tab 1
        Header.Tabs.Add(RecipesTab.RecipeProcess);    // Tab 2

        Header.TabChangedRequested += OnTabChanged;

        // Load Default Tab
        Header.SelectedTab = RecipesTab.RecipeManagement;
        await LoadTabContent(RecipesTab.RecipeManagement);
    }

    private async void OnTabChanged(RecipesTab tab) => await LoadTabContent(tab);

    private async Task LoadTabContent(RecipesTab tab)
    {
        // Cleanup Old Tab
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