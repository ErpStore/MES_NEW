using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Controls.ListHeaderBar;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Service;

namespace MES.Presentation.UI.Modules.Materials.ViewModel;

public enum MaterialsTab { MaterialGroup, MaterialManagement, FeedingPath }

public partial class MaterialsViewModel : BaseViewModel
{
    private readonly IViewModelFactory? _viewModelFactory;
    private readonly IDialogService? _dialogService;
    private readonly IMediator? _mediator;

    public ListHeaderBarViewModel<MaterialsTab>? Header { get; set; }

    [ObservableProperty]
    private BaseViewModel? _currentContentViewModel;

    public MaterialsViewModel(IMediator mediator, IDialogService dialogService , IViewModelFactory viewModelFactory)
    {
        _mediator = mediator;
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
    }

    public override async Task InitializeAsync()
    {
        // Configure the Top Toolbar
        Header = new ListHeaderBarViewModel<MaterialsTab>
        {
            CanAdd = true,
            CanEdit = true,
            CanDelete = true,
            CanRefresh = true,

            // Broadcast Actions to Children
            AddCommand = new RelayCommand(() => SendAction("Add")),
            EditCommand = new RelayCommand(() => SendAction("Edit")),
            DeleteCommand = new RelayCommand(() => SendAction("Delete")),
            RefreshCommand = new RelayCommand(() => SendAction("Refresh"))
        };

        // Add the 3 Tabs
        Header.Tabs.Add(MaterialsTab.MaterialGroup);      // Image 1
        Header.Tabs.Add(MaterialsTab.MaterialManagement); // Image 2
        Header.Tabs.Add(MaterialsTab.FeedingPath);        // Image 3

        Header.TabChangedRequested += OnTabChanged;

        // Load Default Tab
        Header.SelectedTab = MaterialsTab.MaterialGroup;
        await LoadTabContent(MaterialsTab.MaterialGroup);
    }

    private async void OnTabChanged(MaterialsTab tab) => await LoadTabContent(tab);

    private async Task LoadTabContent(MaterialsTab tab)
    {
        // 1. Cleanup Old Tab (Industrial Standard)
        CurrentContentViewModel?.Cleanup();

        BaseViewModel newVm = null;

        switch (tab)
        {
            case MaterialsTab.MaterialGroup:
                // We will build this one first
                newVm = _viewModelFactory.Create<MaterialGroupListViewModel>();
                break;

            case MaterialsTab.MaterialManagement:
                // Placeholder for now
                newVm = _viewModelFactory.Create<MaterialManagementListViewModel>();
                break;

            case MaterialsTab.FeedingPath:
                // Placeholder for now
                newVm = _viewModelFactory.Create<FeedingPathListViewModel>();
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