using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Materials.Dtos;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Modules.Materials.ViewModels;
using MES.Presentation.UI.Service;
using Microsoft.Extensions.Logging;

namespace MES.Presentation.UI.Modules.Materials.ViewModel;

public partial class MaterialGroupListViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly ILogger<MaterialGroupListViewModel>? _logger;

    // Optimized Collection
    public ObservableRangeCollection<MaterialGroupDto> Items { get; } = new();

    [ObservableProperty]
    private MaterialGroupDto? _selectedItem;

    public MaterialGroupListViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory,
        ILogger<MaterialGroupListViewModel>? logger)
    {
        _mediator = mediator;
        _dialogService = dialogService;
        _viewModelFactory = viewModelFactory;
        _logger = logger;        
    }

    public override async Task InitializeAsync()
    {
        _logger?.LogInformation("Initializing MaterialGroup List...");

        WeakReferenceMessenger.Default.Register<HeaderActionMessage>(this, async (r, m) =>
        {
            switch (m.ActionType)
            {
                case "Add": await AddMaterialGroup(); break;
            }
        });

        // 2. Load Data
        await LoadData();
    }

    private async Task LoadData()
    {
        // Simulate DB call for now (Replace with MediatR later)
        var data = await _mediator.Send(new GetAllQuery<MaterialGroupDto>());
        Items.ReplaceRange(data);
    }

    private async Task AddMaterialGroup()
    {
        _logger.LogInformation("Add Material Group");
        await OpenMaterialGroupPopup(null);
    }

    private async Task OpenMaterialGroupPopup(MaterialGroupDto dto)
    {
        _logger?.LogInformation("Open Material Group Popup");
        var vm = _viewModelFactory.Create<MaterialGroupEditViewModel>();

        // B. Initialize (Sets Title, IDs, etc.)
        await vm.InitializeAsync(dto);

        // C. Show Dialog
        bool? result = _dialogService.ShowDialog(vm);

        // D. Refresh Grid if saved
        if (result == true)
        {
            await LoadData();
        }
    }

    [RelayCommand]
    private async Task EditMaterialGroup()
    {
        _logger?.LogInformation("Add User command triggered.");
        var dto = SelectedItem;
        await OpenMaterialGroupPopup(dto);
    }

    [RelayCommand]
    private async Task DeleteMaterialGroup()
    {
        if (SelectedItem == null) return;

        bool confirm = _dialogService.ShowConfirmation(
            $"Are you sure you want to delete Group '{SelectedItem.MaterialName}'?",
            "Confirm Delete");

        if (!confirm) return;

        try
        {
            await _mediator.Send(new DeleteCommand<MaterialGroupDto> { Id = SelectedItem.Id });
            await LoadData();
        }
        catch (System.Exception ex)
        {
            // This catches the 'DeleteBehavior.Restrict' error from Entity Framework
            if (ex.InnerException?.Message.Contains("FK_") == true || ex.Message.Contains("conflicted"))
            {
                _dialogService.ShowMessage(
                    $"Cannot delete Group '{SelectedItem.MaterialName}' because it contains Materials.\nPlease delete or move the Materials first.",
                    "Restricted Action");
            }
            else
            {
                _dialogService.ShowMessage($"Error: {ex.Message}", "Delete Failed");
            }
        }
    }
}