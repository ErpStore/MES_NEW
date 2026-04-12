using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Materials.Dtos;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Service;

namespace MES.Presentation.UI.Modules.Materials.ViewModel;

public partial class FeedingPathListViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;

    public ObservableRangeCollection<FeedingPathDto> Items { get; } = new();

    [ObservableProperty]
    private FeedingPathDto? _selectedItem;

    public FeedingPathListViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory)
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
        var data = await _mediator.Send(new GetAllQuery<FeedingPathDto>());
        Items.ReplaceRange(data);
    }

    private async Task Add() => await OpenEditor(null);

    private async Task Edit()
    {
        if (SelectedItem != null) await OpenEditor(SelectedItem);
    }

    private async Task OpenEditor(FeedingPathDto? dto)
    {
        var vm = _viewModelFactory.Create<FeedingPathEditViewModel>();
        await vm.InitializeAsync(dto);

        bool? result = _dialogService.ShowDialog(vm);
        if (result == true)
        {
            await LoadData();
        }
    }

    private async Task Delete() {
        // 1. Validation: Is something selected?
        if (SelectedItem == null)
        {
            _dialogService.ShowMessage("Please select a Feeding Path to delete.", "No Selection");
            return;
        }

        // 2. Confirmation: Industrial apps always ask twice
        bool confirm = _dialogService.ShowConfirmation(
            $"Are you sure you want to delete Bin {SelectedItem.BinCode}?",
            "Confirm Delete");

        if (!confirm) return;

        try
        {
            // 3. Execute Delete via Mediator
            await _mediator.Send(new DeleteCommand<FeedingPathDto> { Id = SelectedItem.Id });

            // 4. UI Feedback
            await LoadData(); // Refresh the grid
        }
        catch (System.Exception ex)
        {
            // 5. Error Handling
            _dialogService.ShowMessage($"Failed to delete. Error: {ex.Message}", "Delete Error");
        }
    }
}