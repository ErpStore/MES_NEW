using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Recipes.Dtos;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Modules.Recipe.ViewModel;
using MES.Presentation.UI.Service;

namespace MES.Presentation.UI.Modules.Recipe;

public partial class RecipeListViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;

    public ObservableRangeCollection<RecipeDto> Items { get; } = new();

    [ObservableProperty]
    private RecipeDto? _selectedItem;

    public RecipeListViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory)
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
        var data = await _mediator.Send(new GetAllQuery<RecipeDto>());
        Items.ReplaceRange(data);
    }

    private async Task Add() => await OpenEditor(null);

    private async Task Edit()
    {
        if (SelectedItem != null) await OpenEditor(SelectedItem);
    }

    private async Task OpenEditor(RecipeDto? dto)
    {
        var vm = _viewModelFactory.Create<RecipeEditViewModel>();
        await vm.InitializeAsync(dto);

        bool? result = _dialogService.ShowDialog(vm);
        if (result == true)
        {
            await LoadData();
        }
    }

    private async Task Delete()
    {
        if (SelectedItem == null) return;
        if (_dialogService.ShowConfirmation($"Delete recipe {SelectedItem.RecipeCode}?", "Confirm"))
        {
            await _mediator.Send(new DeleteCommand<RecipeDto> { Id = SelectedItem.Id });
            await LoadData();
        }
    }
}