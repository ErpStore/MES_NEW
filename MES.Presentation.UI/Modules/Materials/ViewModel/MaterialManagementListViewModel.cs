using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Materials.Dtos;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Service;

namespace MES.Presentation.UI.Modules.Materials.ViewModel
{
    public partial class MaterialManagementListViewModel : BaseViewModel
    {
        private readonly IMediator _mediator;
        private readonly IDialogService _dialogService;
        private readonly IViewModelFactory _viewModelFactory;

        public ObservableRangeCollection<MaterialDto> Items { get; } = new();

        [ObservableProperty]
        private MaterialDto? _selectedItem;

        public MaterialManagementListViewModel(IMediator mediator, IDialogService dialogService, IViewModelFactory viewModelFactory)
        {
            _mediator = mediator;
            _dialogService = dialogService;
            _viewModelFactory = viewModelFactory;

            WeakReferenceMessenger.Default.Register<HeaderActionMessage>(this, async (r, m) =>
            {
                if (m.ActionType == "Add") await Add();
                else if (m.ActionType == "Edit") await Edit();
                else if (m.ActionType == "Refresh") await LoadData();
                else if (m.ActionType == "Delete") await Delete();
            });
        }

        public override async Task InitializeAsync() => await LoadData();

        private async Task LoadData()
        {
            var data = await _mediator.Send(new GetAllQuery<MaterialDto>());
            Items.ReplaceRange(data);
        }

        private async Task Add() => await OpenEditor(null);

        private async Task Edit()
        {
            if (SelectedItem != null) await OpenEditor(SelectedItem);
        }

        private async Task OpenEditor(MaterialDto? dto)
        {
            var vm = _viewModelFactory.Create<MaterialManagementEditViewModel>();
            await vm.InitializeAsync(dto);

            if (_dialogService.ShowDialog(vm) == true)
            {
                await LoadData();
            }
        }

        private async Task Delete() {

            if (SelectedItem == null)
            {
                _dialogService.ShowMessage("Please select a Material to delete.", "No Selection");
                return;
            }

            bool confirm = _dialogService.ShowConfirmation(
                $"Are you sure you want to delete Material '{SelectedItem.MaterialCode}'?\nThis action cannot be undone.",
                "Confirm Delete");

            if (!confirm) return;

            try
            {
                await _mediator.Send(new DeleteCommand<MaterialDto> { Id = SelectedItem.Id });
                await LoadData();
            }
            catch (System.Exception ex)
            {
                // Friendly Error for Foreign Key Constraint
                if (ex.InnerException?.Message.Contains("FK_") == true || ex.Message.Contains("conflicted"))
                {
                    _dialogService.ShowMessage(
                        "Cannot delete this Material because it is currently assigned to a Feeding Path / Bin.\nPlease remove it from the Feeding Path first.",
                        "Dependency Error");
                }
                else
                {
                    _dialogService.ShowMessage($"Failed to delete. Error: {ex.Message}", "Delete Error");
                }
            }
        }
    }
}
