using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using MES.ApplicationLayer.User.Commands;
using MES.ApplicationLayer.User.Dtos;
using MES.ApplicationLayer.User.Quires;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace MES.Presentation.UI.Modules.UserManagement.ViewModels
{
    public partial class UsersListViewModel : BaseViewModel
    {
        private readonly IMediator _mediator;
        private readonly IDialogService _dialogService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly ILogger<UsersListViewModel>? _logger;

        // =========================
        // DATA
        // =========================
        public ObservableCollection<UserDto> Users { get; } = new();
        private ObservableCollection<UserDto> _allUsers = new();

        // =========================
        // SELECTION
        // =========================
        // FIX 1: Make nullable (?) to stop "must contain non-null value" warning
        [ObservableProperty]
        private UserDto? _selectedUser;

        // =========================
        // SEARCH
        // =========================
        // FIX 2: Initialize with string.Empty to avoid null warnings
        [ObservableProperty]
        private string _searchText = string.Empty;

        // =========================
        // CONSTRUCTOR
        // =========================
        public UsersListViewModel(IMediator mediator, 
            IDialogService dialogService,
            IViewModelFactory viewModelFactory,
            ILogger<UsersListViewModel> logger)
        {
            _mediator = mediator;
            _dialogService = dialogService;
            _logger = logger;

            _logger.LogInformation("Initializing UsersListViewModel...");
            _viewModelFactory = viewModelFactory;
        }

        public override async Task InitializeAsync()
        {
            _logger?.LogInformation("UsersListViewModel InitializeAsync called.");

            WeakReferenceMessenger.Default.Register<HeaderActionMessage>(this, async (r, m) =>
            {
                if (m.ActionType == "Add") await AddUser();
                if (m.ActionType == "Refresh") await Refresh();
            });
            await LoadUsers();
        }

        // =========================
        // COMMANDS
        // =========================

        [RelayCommand]
        private async Task Refresh()
        {
            _logger.LogInformation("Refresh command triggered.");
            await LoadUsers();
        }

        [RelayCommand]
        private async Task AddUser()
        {
            _logger.LogInformation("Add User command triggered.");
            await OpenUserPopup(null);
        }

        [RelayCommand]
        private async Task EditUser()
        {
            if (SelectedUser != null)
            {
                _logger.LogInformation("Edit User command triggered for UserID: {Id}", SelectedUser.Id);
                await OpenUserPopup(SelectedUser);
            }
        }

        [RelayCommand]
        private async Task DeleteUser(UserDto? user)
        {
            if (user == null) return;

            _logger.LogWarning("Requesting deletion for User: {UserName} ({Id})", user.UserName, user.Id);

            var result = MessageBox.Show(
                $"Are you sure you want to permanently delete user '{user.UserName}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _mediator.Send(new DeleteUserCommand { Id = user.Id });
                    _logger.LogInformation("User {Id} deleted successfully.", user.Id);
                    await LoadUsers();
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete user {Id}", user.Id);
                    MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                _logger.LogInformation("Deletion cancelled by user.");
            }
        }

        // =========================
        // PRIVATE METHODS
        // =========================

        private async Task OpenUserPopup(UserDto? userToEdit)
        {
            var vm = _viewModelFactory.Create<UserEditViewModel>();

            // 2. Initialize: Pass data (Edit Mode vs Add Mode)
            vm.Initialize(userToEdit);

            try 
            { 
                _logger.LogInformation("Showing UserEditView dialog.");
                bool? result = _dialogService.ShowDialog(vm);

                if (result == true)
                {
                    _logger.LogInformation("Dialog returned True (Saved). Reloading list.");
                    await LoadUsers();
                }
                else
                {
                    _logger.LogInformation("Dialog returned False or was closed (Cancelled). No action taken.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening User Popup.");
                _dialogService.ShowMessage("Could not open editor.", "Error");
            }
        }

        private async Task LoadUsers()
        {
            try
            {
                _logger.LogDebug("Sending GetUsersQuery...");
                var userList = await _mediator.Send(new GetUsersQuery());

                _allUsers = new ObservableCollection<UserDto>(userList);
                _logger.LogInformation("Loaded {Count} users from database.", _allUsers.Count);

                ApplyFilter();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load users list.");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Users.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var user in _allUsers) Users.Add(user);
            }
            else
            {
                // FIX 9: Handle potential nulls in User properties (e.g. u.Email might be null in DB)
                var filtered = _allUsers.Where(u =>
                    (u.UserName != null && u.UserName.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)) ||
                    (u.Email != null && u.Email.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase))
                );

                foreach (var user in filtered)
                    Users.Add(user);
            }
        }
    }
}