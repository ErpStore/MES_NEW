using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.User.Dtos;
using MES.ApplicationLayer.User.Handlers;
using MES.Presentation.UI.Common;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace MES.Presentation.UI.Modules.UserManagement.ViewModels;

public partial class UserEditViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserEditViewModel>? _logger;
    public string? WindowTitle { get; set; }

    [ObservableProperty]
    private int _id;

    // ★ User Name is Mandatory
    [ObservableProperty]
    [Required(ErrorMessage = "User Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    private string? _userName;

    [ObservableProperty]
    private string? _firstName;

    [ObservableProperty]
    private string? _lastName;

    [ObservableProperty]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    private string? _email; // Optional, but if entered, must be valid email

    [ObservableProperty]
    private string? _mobile;

    // ★ Department is Mandatory
    [ObservableProperty]
    private ObservableCollection<UserGroupDto> _departments = new();

    [ObservableProperty]
    [Required(ErrorMessage = "Please select a department")]
    private UserGroupDto? _selectedDepartment;

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private bool _enablePasswordExpiry;

    [ObservableProperty]
    private DateTime? _passwordExpiryDate;

    // ==========================
    // PASSWORD PROPERTIES
    // ==========================
    // We bind these to the PasswordBox (using a helper or simple binding if you use a behavior)
    // For simplicity, we will validate these manually in the Save command since PasswordBox is tricky.
    public string? PasswordInput { get; set; }
    public string? ConfirmPasswordInput { get; set; }

    public UserEditViewModel(IMediator mediator, ILogger<UserEditViewModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async void Initialize(UserDto? user)
    {
        var groups = await _mediator.Send(new GetAllQuery<UserGroupDto>());
        Departments = new ObservableCollection<UserGroupDto>(groups);

        if (user == null)
        {
            // MODE: ADD NEW
            WindowTitle = "Add New User";
            Id = 0;
            IsActive = true; // Default to active
            EnablePasswordExpiry = false;
            PasswordExpiryDate = DateTime.Now.AddMonths(3); // Default
            SelectedDepartment = null;
        }
        else
        {
            // MODE: EDIT EXISTING
            WindowTitle = "Edit User";
            Id = user.Id;
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Mobile = user.Mobile;
            IsActive = user.IsActive;
            EnablePasswordExpiry = user.EnablePasswordExpiry;

            // Handle nullable dates safely
            PasswordExpiryDate = user.PasswordExpiryDate ?? DateTime.Now;

            if (user.UserGroupId.HasValue && Departments.Any(d => d.Id == user.UserGroupId.Value))
            {
                SelectedDepartment = Departments.First(d => d.Id == user.UserGroupId.Value);
            }
        }

        ClearErrors();
    }

    [RelayCommand]
    private async Task Save()
    {
        _logger.LogInformation("Save command triggered.");
        ValidateAllProperties();

        if (HasErrors)
        {
            // Get the first error message to show the user
            var firstError = GetErrors(null).First();
            _logger.LogWarning("Validation failed. First error: {Error}", firstError.ErrorMessage);
            MessageBox.Show(firstError.ErrorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // 3. CHECK PASSWORD MANUALLY (Custom Logic)
        if (Id == 0) // Only validate password for NEW users
        {
            if (string.IsNullOrWhiteSpace(PasswordInput))
            {
                _logger.LogWarning("Validation failed: Password empty for new user.");
                MessageBox.Show("Password is required for new users.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PasswordInput != ConfirmPasswordInput)
            {
                _logger.LogWarning("Validation failed: Passwords do not match (Add Mode).");
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        else
        {
            // For Edit Mode: If user typed a password, check if they match
            if (!string.IsNullOrEmpty(PasswordInput) && PasswordInput != ConfirmPasswordInput)
            {
                _logger.LogWarning("Validation failed: Passwords do not match (Edit Mode).");
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        await SaveToDb();
    }

    private async Task SaveToDb()
    {
        try
        {
            _logger.LogInformation("Preparing SaveUserCommand DTO...");
            var command = new SaveUserCommand
            {
                Id = Id,
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Mobile = Mobile,
                UserGroupId = SelectedDepartment?.Id ?? 0,
                IsActive = IsActive,
                EnablePasswordExpiry = EnablePasswordExpiry,
                PasswordExpiryDate = PasswordExpiryDate,
                Password = PasswordInput
            };

            _logger.LogInformation("Sending SaveUserCommand to Mediator...");
            await _mediator.Send(command);

            _logger.LogInformation("User saved successfully. Closing dialog.");
            CloseAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while saving user.");
            MessageBox.Show($"Error saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _logger.LogInformation("User cancelled operation.");
        CloseAction?.Invoke(false);
    }
}