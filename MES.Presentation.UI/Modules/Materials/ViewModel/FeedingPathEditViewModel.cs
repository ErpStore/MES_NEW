using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Materials.Dtos;
using MES.Presentation.UI.Common;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MES.Presentation.UI.Modules.Materials.ViewModel;

public partial class FeedingPathEditViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<FeedingPathEditViewModel> _logger;

    [ObservableProperty]
    private string _windowTitle = "Feeding Path";

    // --- FORM PROPERTIES ---
    [ObservableProperty] private int _id;

    [ObservableProperty]
    [Required(ErrorMessage = "Bin Number is required")]
    private string _binNumber = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Bin Code is required")]
    private string _binCode = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Max Capacity is required")]
    private double _maxCapacity;

    [ObservableProperty]
    private double _currentStock;

    [ObservableProperty]
    private DateTime? _filledDate;

    [ObservableProperty]
    private DateTime? _expiryDate;

    [ObservableProperty]
    private string? _description;

    // --- DROPDOWN LOGIC ---
    public ObservableCollection<MaterialDto> Materials { get; } = new();

    [ObservableProperty]
    [Required(ErrorMessage = "Material is required")]
    private MaterialDto? _selectedMaterial;

    public FeedingPathEditViewModel(IMediator mediator, ILogger<FeedingPathEditViewModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task InitializeAsync(FeedingPathDto? dto)
    {
        // 1. Fetch Materials for Dropdown
        var materialsList = await _mediator.Send(new GetAllQuery<MaterialDto>());
        Materials.Clear();
        foreach (var m in materialsList) Materials.Add(m);

        // 2. Map Data
        if (dto == null)
        {
            WindowTitle = "Add Feeding Path / Bin";
            Id = 0;
            FilledDate = DateTime.Now; // Default to today for new bins
        }
        else
        {
            WindowTitle = "Edit Feeding Path / Bin";
            Id = dto.Id;
            BinNumber = dto.BinNumber;
            BinCode = dto.BinCode;
            MaxCapacity = dto.MaxCapacity;
            CurrentStock = dto.CurrentStock;
            FilledDate = dto.FilledDate;
            ExpiryDate = dto.ExpiryDate;
            Description = dto.Description;

            SelectedMaterial = Materials.FirstOrDefault(m => m.Id == dto.MaterialId);
        }

        ClearErrors();
    }

    [RelayCommand]
    private async Task Save()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        try
        {
            var dto = new FeedingPathDto
            {
                Id = Id,
                BinNumber = BinNumber,
                BinCode = BinCode,
                MaterialId = SelectedMaterial!.Id, // Validated
                MaxCapacity = MaxCapacity,
                CurrentStock = CurrentStock,
                FilledDate = FilledDate,
                ExpiryDate = ExpiryDate,
                Description = Description
            };

            await _mediator.Send(new SaveCommand<FeedingPathDto> { Data = dto });

            _logger.LogInformation("Feeding Path {Code} saved successfully.", BinCode);
            CloseAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save Feeding Path.");
        }
    }

    [RelayCommand]
    private void Cancel() => CloseAction?.Invoke(false);
}