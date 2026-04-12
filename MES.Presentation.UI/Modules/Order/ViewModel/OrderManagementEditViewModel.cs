using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Orders.DTO;
using MES.ApplicationLayer.Recipes.Dtos;
using MES.Presentation.UI.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MES.Presentation.UI.Modules.Order.ViewModel;

public partial class OrderManagementEditViewModel : BaseViewModel
{
    private readonly IMediator _mediator;

    [ObservableProperty] private string _windowTitle = "Production Order";

    [ObservableProperty] private int _id;

    [ObservableProperty][Required] private int _serialNumber;
    [ObservableProperty][Required] private int _setBatch;

    [ObservableProperty] private int _actualBatch;
    [ObservableProperty] private string _status = "Not Started";
    [ObservableProperty] private bool _isReleased;
    [ObservableProperty] private DateTime? _batchStart;
    [ObservableProperty] private DateTime? _batchEnd;
    [ObservableProperty] private string? _description;

    public ObservableCollection<RecipeDto> Recipes { get; } = new();

    [ObservableProperty]
    [Required(ErrorMessage = "Recipe is required")]
    private RecipeDto? _selectedRecipe;

    public OrderManagementEditViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task InitializeAsync(ProductionOrderDto? dto)
    {
        var recipesList = await _mediator.Send(new GetAllQuery<RecipeDto>());
        Recipes.Clear();
        foreach (var r in recipesList) Recipes.Add(r);

        if (dto == null)
        {
            WindowTitle = "Add Production Order";
            Id = 0;
            Status = "Not Started";
        }
        else
        {
            WindowTitle = "Edit Production Order";
            Id = dto.Id;
            SerialNumber = dto.SerialNumber;
            SetBatch = dto.SetBatch;
            ActualBatch = dto.ActualBatch;
            Status = dto.Status;
            IsReleased = dto.IsReleased;
            BatchStart = dto.BatchStart;
            BatchEnd = dto.BatchEnd;
            Description = dto.Description;

            SelectedRecipe = Recipes.FirstOrDefault(r => r.Id == dto.RecipeId);
        }
        ClearErrors();
    }

    [RelayCommand]
    private async Task Save()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        var dto = new ProductionOrderDto
        {
            Id = Id,
            SerialNumber = SerialNumber,
            RecipeId = SelectedRecipe!.Id,
            SetBatch = SetBatch,
            ActualBatch = ActualBatch,
            Status = Status,
            IsReleased = IsReleased,
            BatchStart = BatchStart,
            BatchEnd = BatchEnd,
            Description = Description
        };

        await _mediator.Send(new SaveCommand<ProductionOrderDto> { Data = dto });
        CloseAction?.Invoke(true);
    }

    [RelayCommand] private void Cancel() => CloseAction?.Invoke(false);
}