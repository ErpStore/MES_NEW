using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Recipes.Dtos;
using MES.Presentation.UI.Common;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MES.Presentation.UI.Modules.Recipe.ViewModel;

public partial class RecipeEditViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<RecipeEditViewModel> _logger;

    [ObservableProperty]
    private string _windowTitle = "Recipe Management";

    // --- FORM PROPERTIES ---
    [ObservableProperty] private int _id;

    [ObservableProperty]
    [Required(ErrorMessage = "Recipe Name is required")]
    private string _recipeName = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Recipe Code is required")]
    private string _recipeCode = string.Empty;

    [ObservableProperty]
    private int _version = 1;

    [ObservableProperty]
    private double _batchWeight;

    [ObservableProperty]
    private bool _isBlocked;

    [ObservableProperty]
    private string? _description;

    // --- DROPDOWN LOGIC FOR 'TYPE' ---
    public ObservableCollection<string> RecipeTypes { get; } = new ObservableCollection<string>
    {
        "Master",
        "Final",
        "Intermediate"
    };

    [ObservableProperty]
    [Required(ErrorMessage = "Recipe Type is required")]
    private string _selectedType = "Master";

    public RecipeEditViewModel(IMediator mediator, ILogger<RecipeEditViewModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task InitializeAsync(RecipeDto? dto)
    {
        if (dto == null)
        {
            WindowTitle = "Add Recipe";
            Id = 0;
            Version = 1;
            SelectedType = "Master";
        }
        else
        {
            WindowTitle = "Edit Recipe";
            Id = dto.Id;
            RecipeName = dto.RecipeName;
            RecipeCode = dto.RecipeCode;
            Version = dto.Version;
            SelectedType = dto.Type;
            BatchWeight = dto.BatchWeight;
            IsBlocked = dto.IsBlocked;
            Description = dto.Description;
        }

        ClearErrors();
        await Task.CompletedTask; // Keep async signature happy
    }

    [RelayCommand]
    private async Task Save()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        try
        {
            var dto = new RecipeDto
            {
                Id = Id,
                RecipeName = RecipeName,
                RecipeCode = RecipeCode,
                Version = Version,
                Type = SelectedType,
                BatchWeight = BatchWeight,
                IsBlocked = IsBlocked,
                Description = Description
                // CreatedDate and ModifiedDate are handled by the Handler
            };

            await _mediator.Send(new SaveCommand<RecipeDto> { Data = dto });
            _logger.LogInformation("Recipe {Code} saved successfully.", RecipeCode);
            CloseAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save Recipe.");
        }
    }

    [RelayCommand]
    private void Cancel() => CloseAction?.Invoke(false);
}