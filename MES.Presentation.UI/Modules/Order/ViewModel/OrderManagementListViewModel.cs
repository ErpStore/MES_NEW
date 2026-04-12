using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Orders.DTO;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Service;

namespace MES.Presentation.UI.Modules.Order.ViewModel;

public partial class OrderManagementListViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;

    public ObservableRangeCollection<ProductionOrderDto> Items { get; } = new();

    [ObservableProperty]
    private ProductionOrderDto? _selectedItem;

    public OrderManagementListViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory)
    {
        _mediator = mediator;
        _dialogService = dialogService;
        _viewModelFactory = viewModelFactory;

        WeakReferenceMessenger.Default.Register<HeaderActionMessage>(this, async (r, m) =>
        {
            switch (m.ActionType)
            {
                case "Add": await Add(); break;
                case "Edit": await Edit(); break;
                case "Refresh": await LoadData(); break;
                case "Delete": await Delete(); break;
            }
        });
    }

    public override async Task InitializeAsync() => await LoadData();

    private async Task LoadData()
    {
        var data = await _mediator.Send(new GetAllQuery<ProductionOrderDto>());
        Items.ReplaceRange(data);
    }

    private async Task Add() => await OpenEditor(null);

    private async Task Edit()
    {
        if (SelectedItem != null) await OpenEditor(SelectedItem);
    }

    private async Task OpenEditor(ProductionOrderDto? dto)
    {
        var vm = _viewModelFactory.Create<OrderManagementEditViewModel>();
        await vm.InitializeAsync(dto);
        if (_dialogService.ShowDialog(vm) == true) await LoadData();
    }

    private async Task Delete()
    {
        if (SelectedItem == null) return;
        if (_dialogService.ShowConfirmation($"Delete Order S.No. {SelectedItem.SerialNumber}?", "Confirm"))
        {
            await _mediator.Send(new DeleteCommand<ProductionOrderDto> { Id = SelectedItem.Id });
            await LoadData();
        }
    }
}