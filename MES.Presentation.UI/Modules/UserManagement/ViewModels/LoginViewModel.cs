using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.User.Dtos;
using MES.ApplicationLayer.User.Quires;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace MES.Presentation.UI.Modules.UserManagement.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<LoginViewModel>? _logger;

        // ========================
        // DATA
        // ========================
        public ObservableCollection<UserDto> Users { get; } = new();

        [ObservableProperty]
        private UserDto? _selectedUser;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private bool _hasError;

        // ========================
        // CONSTRUCTOR
        // ========================
        public LoginViewModel(IMediator mediator, ICurrentUserService currentUserService, ILogger<LoginViewModel>? logger = null)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public override async Task InitializeAsync()
        {
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                var result = await _mediator.Send(new GetUsersQuery());
                Users.Clear();
                foreach (var u in result.Where(u => u.IsActive))
                    Users.Add(u);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load users for login.");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            // Filter is handled in the view via binding to filtered list
        }

        [RelayCommand]
        private async Task Login()
        {
            if (SelectedUser == null)
            {
                ErrorMessage = "Please select a user.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your password.";
                HasError = true;
                return;
            }

            try
            {
                HasError = false;
                ErrorMessage = null;

                var result = await _mediator.Send(new LoginQuery
                {
                    UserName = SelectedUser.UserName!,
                    Password = Password
                });

                if (result.Success && result.User != null)
                {
                    _currentUserService.Login(result.User, result.Rights);
                    _logger?.LogInformation("User {UserName} logged in.", result.User.UserName);
                    CloseAction?.Invoke(true);
                }
                else
                {
                    ErrorMessage = result.ErrorMessage ?? "Login failed.";
                    HasError = true;
                    Password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Login error.");
                ErrorMessage = "An error occurred. Please try again.";
                HasError = true;
            }
        }

        [RelayCommand]
        private void Logout()
        {
            _currentUserService.Logout();
            CloseAction?.Invoke(false);
        }
    }
}
