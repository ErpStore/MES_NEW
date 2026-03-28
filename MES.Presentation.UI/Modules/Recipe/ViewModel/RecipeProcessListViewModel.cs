using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Recipes.Dtos;
using MES.ApplicationLayer.Recipes.Queries;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace MES.Presentation.UI.Modules.Recipe.ViewModel
{
    public partial class RecipeProcessListViewModel : BaseViewModel
    {
        private readonly IMediator _mediator;
        private readonly IDialogService _dialogService;
        private readonly IViewModelFactory _viewModelFactory;

        // --- LEFT PANEL: MASTER LIST ---

        // The raw list of all recipes
        private ObservableCollection<RecipeDto> _allRecipes = new();

        // The filtered view bound to the UI list
        public ICollectionView FilteredRecipesView { get; }

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private RecipeDto? _selectedRecipe;


        // --- RIGHT PANEL: DETAIL LIST ---
        public ObservableRangeCollection<RecipeItemDto> Items { get; } = new();

        [ObservableProperty]
        private RecipeItemDto? _selectedItem;

        public RecipeProcessListViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory)
        {
            _mediator = mediator;
            _dialogService = dialogService;
            _viewModelFactory = viewModelFactory;

            // Initialize the filterable view
            FilteredRecipesView = CollectionViewSource.GetDefaultView(_allRecipes);
            FilteredRecipesView.Filter = FilterRecipes;

            WeakReferenceMessenger.Default.Register<HeaderActionMessage>(this, async (r, m) =>
            {
                switch (m.ActionType)
                {
                    case "Add": await Add(); break;
                    case "Edit": await Edit(); break;
                    case "Refresh": await LoadMasterList(); break;
                    case "Delete": await Delete(); break;
                }
            });
        }

        public override async Task InitializeAsync()
        {
            await LoadMasterList();
        }

        // Load the list of recipes for the left panel
        private async Task LoadMasterList()
        {
            var recipes = await _mediator.Send(new GetAllQuery<RecipeDto>());
            _allRecipes.Clear();
            foreach (var r in recipes) _allRecipes.Add(r);

            // Refresh the filter in case search text exists
            FilteredRecipesView.Refresh();
        }

        // Filter logic for the search bar
        private bool FilterRecipes(object item)
        {
            if (item is not RecipeDto recipe) return false;
            if (string.IsNullOrWhiteSpace(SearchText)) return true;

            // Filter by Recipe Code (case-insensitive)
            return recipe.RecipeCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }

        // Re-apply filter when user types
        partial void OnSearchTextChanged(string value) => FilteredRecipesView.Refresh();

        // When a Recipe is clicked in the left panel, load its items in the right panel
        partial void OnSelectedRecipeChanged(RecipeDto? value)
        {
            _ = LoadDetailList();
        }

        private async Task LoadDetailList()
        {
            if (SelectedRecipe == null)
            {
                Items.Clear();
                return;
            }

            // Fetch items specifically for the selected Recipe
            var data = await _mediator.Send(new GetRecipeItemsQuery { RecipeId = SelectedRecipe.Id });
            Items.ReplaceRange(data);
        }

        // --- ACTION METHODS ---

        private async Task Add()
        {
            if (SelectedRecipe == null)
            {
                _dialogService.ShowMessage("Please select a Recipe from the list first.", "Select Recipe");
                return;
            }
            await OpenEditor(null);
        }

        private async Task Edit()
        {
            if (SelectedItem != null) { } // await OpenEditor(SelectedItem);
        }

        private async Task Delete()
        {
            if (SelectedItem == null) return;
            if (_dialogService.ShowConfirmation($"Delete item {SelectedItem.MaterialCode}?", "Confirm"))
            {
                await _mediator.Send(new DeleteCommand<RecipeItemDto> { Id = SelectedItem.Id });
                await LoadDetailList();
            }
        }

        private async Task OpenEditor(RecipeItemDto? dto)
        {
            var vm = _viewModelFactory.Create<RecipeProcessEditViewModel>();

            // Pass the currently selected Recipe's ID to the child item
            await vm.InitializeAsync(SelectedRecipe!.Id, dto);

            if (_dialogService.ShowDialog(vm) == true)
            {
                // Refresh the right-side grid after saving
                await LoadDetailList();
            }
        }
    }
}
