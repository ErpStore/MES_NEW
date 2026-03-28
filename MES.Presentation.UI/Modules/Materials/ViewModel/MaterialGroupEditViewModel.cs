using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.Common; // For SaveCommand<T>
using MES.ApplicationLayer.Materials.Dtos;
using MES.Presentation.UI.Common;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MES.Presentation.UI.Modules.Materials.ViewModels
{
    public partial class MaterialGroupEditViewModel : BaseViewModel
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MaterialGroupEditViewModel> _logger;

        [ObservableProperty]
        private string _windowTitle = "Material Group";

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        [Required(ErrorMessage = "Group Name is required")]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        public MaterialGroupEditViewModel(IMediator mediator, ILogger<MaterialGroupEditViewModel> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task InitializeAsync(MaterialGroupDto? dto)
        {
            try
            {
                _logger.LogInformation("Loading the InitializeAsync");
                if (dto == null)
                {
                    // MODE: ADD
                    WindowTitle = "Add Material Group";
                    Id = 0;
                    Name = string.Empty;
                    Description = string.Empty;
                }
                else
                {
                    // MODE: EDIT
                    WindowTitle = "Edit Material Group";
                    Id = dto.Id;
                    Name = dto.MaterialName;
                    Description = dto.MaterialDescription;
                }
                ClearErrors();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            ValidateAllProperties();
            if (HasErrors) return;

            try
            {
                var dto = new MaterialGroupDto
                {
                    Id = Id,
                    MaterialName = Name,
                    MaterialDescription = Description
                };

                _logger.LogInformation("Saving Material Group: {Name}", Name);

                // GENERIC SAVE COMMAND
                await _mediator.Send(new SaveCommand<MaterialGroupDto> { Data = dto });

                CloseAction?.Invoke(true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to save Material Group");
                // In real app, show error dialog here
            }
        }

        [RelayCommand]
        private void Cancel() => CloseAction?.Invoke(false);
    }
}