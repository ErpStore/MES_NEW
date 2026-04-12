using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Materials.Dtos;
using MES.Presentation.UI.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MES.Presentation.UI.Modules.Materials.ViewModel;

public partial class MaterialManagementEditViewModel : BaseViewModel
{
    private readonly IMediator _mediator;

    [ObservableProperty] private string _windowTitle;

    // Form Fields
    [ObservableProperty] private int _id;
    [ObservableProperty][Required] private string _materialCode;
    [ObservableProperty][Required] private string _name;
    [ObservableProperty] private string _unit = "kg"; // Default
    [ObservableProperty] private double _density;
    [ObservableProperty] private double _minLevel;
    [ObservableProperty] private int _shelfLifeDays;
    [ObservableProperty] private bool _isBlocked;
    [ObservableProperty] private string? _handlingInfo;
    [ObservableProperty] private string? _manufacturer;
    [ObservableProperty] private string? _description;

    // DROPDOWN LOGIC
    public ObservableCollection<MaterialGroupDto> MaterialGroups { get; } = new();

    [ObservableProperty]
    [Required]
    private MaterialGroupDto? _selectedGroup;

    public MaterialManagementEditViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task InitializeAsync(MaterialDto? dto)
    {
        var groups = await _mediator.Send(new GetAllQuery<MaterialGroupDto>());
        MaterialGroups.Clear();
        foreach (var g in groups) MaterialGroups.Add(g);

        if (dto == null)
        {
            WindowTitle = "Add Material";
            Id = 0;
        }
        else
        {
            WindowTitle = "Edit Material";
            Id = dto.Id;
            MaterialCode = dto.MaterialCode;
            Name = dto.Name;
            Unit = dto.Unit;
            Density = dto.Density;
            MinLevel = dto.MinLevel;
            ShelfLifeDays = dto.ShelfLifeDays;
            IsBlocked = dto.IsBlocked;
            HandlingInfo = dto.HandlingInfo;
            Manufacturer = dto.Manufacturer;
            Description = dto.Description;

            // Pre-select the correct group in dropdown
            SelectedGroup = MaterialGroups.FirstOrDefault(g => g.Id == dto.MaterialGroupId);
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        var dto = new MaterialDto
        {
            Id = Id,
            MaterialCode = MaterialCode,
            Name = Name,
            MaterialGroupId = SelectedGroup.Id, // Use selected ID
            Unit = Unit,
            Density = Density,
            MinLevel = MinLevel,
            ShelfLifeDays = ShelfLifeDays,
            IsBlocked = IsBlocked,
            HandlingInfo = HandlingInfo,
            Manufacturer = Manufacturer,
            Description = Description
        };

        await _mediator.Send(new SaveCommand<MaterialDto> { Data = dto });
        CloseAction?.Invoke(true);
    }

    [RelayCommand] private void Cancel() => CloseAction?.Invoke(false);
}