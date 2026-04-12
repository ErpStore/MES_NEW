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
        private readonly ICurrentUserService? _currentUserService;

    public ListHeaderBarViewModel<MaterialsTab>? Header { get; set; }

    [ObservableProperty]
    private BaseViewModel? _currentContentViewModel;

        public MaterialsViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory,
            ICurrentUserService? currentUserService = null)
        {
            _mediator = mediator;
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            _currentUserService = currentUserService;
        }

        public override async Task InitializeAsync()
        {
            // Default tab is MaterialGroup – check its rights
            var rights = _currentUserService?.GetRights(ScreenKeys.MaterialGroup);

            // Configure the Top Toolbar
            Header = new ListHeaderBarViewModel<MaterialsTab>
            {
                CanAdd = rights?.CanAdd ?? true,
                CanEdit = rights?.CanEdit ?? true,
                CanDelete = rights?.CanDelete ?? true,
                CanRefresh = true,

            // Broadcast Actions to Children
            AddCommand = new RelayCommand(() => SendAction("Add")),
            EditCommand = new RelayCommand(() => SendAction("Edit")),
            DeleteCommand = new RelayCommand(() => SendAction("Delete")),
            RefreshCommand = new RelayCommand(() => SendAction("Refresh"))
        };

            // Add the 3 Tabs
            Header.Tabs.Add(MaterialsTab.MaterialGroup);
            Header.Tabs.Add(MaterialsTab.MaterialManagement);
            Header.Tabs.Add(MaterialsTab.FeedingPath);

        Header.TabChangedRequested += OnTabChanged;

        // Load Default Tab
        Header.SelectedTab = MaterialsTab.MaterialGroup;
        await LoadTabContent(MaterialsTab.MaterialGroup);
    }

        private async void OnTabChanged(MaterialsTab tab)
        {
            // Update toolbar rights when tab changes
            if (Header != null && _currentUserService != null)
            {
                var screenKey = tab switch
                {
                    MaterialsTab.MaterialGroup => ScreenKeys.MaterialGroup,
                    MaterialsTab.MaterialManagement => ScreenKeys.MaterialManagement,
                    MaterialsTab.FeedingPath => ScreenKeys.FeedingPath,
                    _ => ScreenKeys.MaterialGroup
                };
                var rights = _currentUserService.GetRights(screenKey);
                Header.CanAdd = rights?.CanAdd ?? true;
                Header.CanEdit = rights?.CanEdit ?? true;
                Header.CanDelete = rights?.CanDelete ?? true;
            }

            await LoadTabContent(tab);
        }

    private async Task LoadTabContent(MaterialsTab tab)
    {
        // 1. Cleanup Old Tab (Industrial Standard)
        CurrentContentViewModel?.Cleanup();

        BaseViewModel? newVm = null;

            switch (tab)
            {
                case MaterialsTab.MaterialGroup:
                    newVm = _viewModelFactory.Create<MaterialGroupListViewModel>();
                    break;

                case MaterialsTab.MaterialManagement:
                    newVm = _viewModelFactory.Create<MaterialManagementListViewModel>();
                    break;

                case MaterialsTab.FeedingPath:
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