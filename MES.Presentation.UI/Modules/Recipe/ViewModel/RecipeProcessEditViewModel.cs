using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Materials.Dtos;
using MES.ApplicationLayer.Recipes.Dtos;
using MES.Presentation.UI.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MES.Presentation.UI.Modules.Recipe.ViewModel;

public partial class RecipeProcessEditViewModel : BaseViewModel
{
    private readonly IMediator _mediator;

    [ObservableProperty] private string _windowTitle = "Recipe Item";

    [ObservableProperty] private int _id;
    [ObservableProperty] private int _recipeId;

    [ObservableProperty][Required] private int _serialNumber;
    [ObservableProperty][Required] private double _weight;
    [ObservableProperty] private double _tolerancePositive;
    [ObservableProperty] private double _toleranceNegative;
    [ObservableProperty] private string? _description;

    public ObservableCollection<MaterialDto> Materials { get; } = new();

    [ObservableProperty]
    [Required(ErrorMessage = "Material is required")]
    private MaterialDto? _selectedMaterial;

    public RecipeProcessEditViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task InitializeAsync(int recipeId, RecipeItemDto? dto)
    {
        RecipeId = recipeId;

        var materials = await _mediator.Send(new GetAllQuery<MaterialDto>());
        Materials.Clear();
        foreach (var m in materials) Materials.Add(m);

        if (dto == null)
        {
            WindowTitle = "Add Recipe Item";
            Id = 0;
            SerialNumber = 1;
        }
        else
        {
            WindowTitle = "Edit Recipe Item";
            Id = dto.Id;
            SerialNumber = dto.SerialNumber;
            Weight = dto.Weight;
            TolerancePositive = dto.TolerancePositive;
            ToleranceNegative = dto.ToleranceNegative;
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

        var dto = new RecipeItemDto
        {
            Id = Id,
            RecipeId = RecipeId,
            SerialNumber = SerialNumber,
            MaterialId = SelectedMaterial!.Id,
            Weight = Weight,
            TolerancePositive = TolerancePositive,
            ToleranceNegative = ToleranceNegative,
            Description = Description
        };

        await _mediator.Send(new SaveCommand<RecipeItemDto> { Data = dto });
        CloseAction?.Invoke(true);
    }

    [RelayCommand] private void Cancel() => CloseAction?.Invoke(false);
}