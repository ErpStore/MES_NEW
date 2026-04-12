using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Controls.ListHeaderBar;
using MES.Presentation.UI.Messages;

namespace MES.Presentation.UI.Modules.Order.ViewModel;

public enum OrdersTab { OrderManagement }
public partial class OrderManagementViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;

    public ListHeaderBarViewModel<OrdersTab>? Header { get; set; }

    [ObservableProperty]
    private BaseViewModel? _currentContentViewModel;

    public OrderManagementViewModel(IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public override async Task InitializeAsync()
    {
        Header = new ListHeaderBarViewModel<OrdersTab>
        {
            CanAdd = true,
            CanEdit = true,
            CanDelete = true,
            CanRefresh = true,
            AddCommand = new RelayCommand(() => SendAction("Add")),
            EditCommand = new RelayCommand(() => SendAction("Edit")),
            DeleteCommand = new RelayCommand(() => SendAction("Delete")),
            RefreshCommand = new RelayCommand(() => SendAction("Refresh"))
        };

        Header.Tabs.Add(OrdersTab.OrderManagement);
        Header.TabChangedRequested += OnTabChanged;

        Header.SelectedTab = OrdersTab.OrderManagement;
        await LoadTabContent(OrdersTab.OrderManagement);
    }

    private async void OnTabChanged(OrdersTab tab) => await LoadTabContent(tab);

    private async Task LoadTabContent(OrdersTab tab)
    {
        CurrentContentViewModel?.Cleanup();
        BaseViewModel newVm = null;

        if (tab == OrdersTab.OrderManagement)
        {
            newVm = _viewModelFactory.Create<OrderManagementListViewModel>();
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