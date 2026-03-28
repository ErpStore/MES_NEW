using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.User.Dtos;
using MES.Presentation.UI.Common;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace MES.Presentation.UI.Modules.UserManagement.ViewModels
{
    public partial class UserGroupEditViewModel : BaseViewModel
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserEditViewModel>? _logger;

        [ObservableProperty]
        private string? _windowTitle;

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        [Required(ErrorMessage = "Name is required")]
        public string? _name;

        [ObservableProperty]
        public string? _description;

        public UserGroupEditViewModel(IMediator mediator, ILogger<UserEditViewModel>? logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task InitializeAsync(UserGroupDto userGroupDto)
        {

            if (userGroupDto == null)
            {
                // MODE: ADD NEW
                WindowTitle = "Add New User Group";
                Id = 0;
                Name = string.Empty;
                Description = string.Empty;
            }
            else
            {
                // MODE: EDIT EXISTING
                WindowTitle = "Edit User Group";
                Id = userGroupDto.Id;
                Name = userGroupDto.Name;
                Description = userGroupDto.Description;
            }

            ClearErrors();
        }

        private void ValidateAllProperties()
        {
            ClearErrors();
            ValidateProperty(Name, nameof(Name));
            // Add more property validations here if needed
        }

        [RelayCommand]
        private async Task Save() // 1. CHANGE 'void' TO 'async Task'
        {
            // 1. Validation
            ValidateAllProperties();

            if (HasErrors)
            {
                _logger?.LogWarning("Validation errors exist. Cannot save User Group.");
                // Optional: Show a dialog if you want the user to know why
                // MessageBox.Show("Please fix the errors before saving.", "Validation Error");
                return;
            }

            try
            {
                // 2. Map ViewModel to DTO
                var userGroupDto = new UserGroupDto
                {
                    Id = this.Id,
                    Name = this.Name,
                    Description = this.Description
                };

                _logger?.LogInformation("Saving User Group: {Name}", Name);

                await _mediator.Send(new SaveCommand<UserGroupDto>
                {
                    Data = userGroupDto
                });

                _logger?.LogInformation("User Group saved successfully.");

                CloseAction?.Invoke(true);
            }
            catch (System.Exception ex)
            {
                // 5. Error Handling
                _logger?.LogError(ex, "Failed to save User Group.");

                // In a real app, you inject IDialogService here to show the error
                System.Windows.MessageBox.Show($"Error saving data: {ex.Message}", "Error");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            _logger.LogInformation("User cancelled operation.");
            CloseAction?.Invoke(false);
        }



    }
}
