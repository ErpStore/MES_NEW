using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.User.Dtos;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace MES.Presentation.UI.Modules.UserManagement.ViewModels;

public partial class UserListDepartmentsViewModel : BaseViewModel
{

    private readonly IMediator? _mediator;
    private readonly IDialogService? _dialogService;
    private readonly IViewModelFactory? _viewModelFactory;
    private readonly ILogger<UsersListViewModel>? _logger;
    private bool _isInitialized;

    [ObservableProperty]
    private ObservableCollection<UserGroupDto> _userGroups = new();

    [ObservableProperty]
    private UserGroupDto? _selectedGroup;

    [RelayCommand]
    private async Task EditGroup()
    {
        if (SelectedGroup != null)
        {
            await OpenUserGroupPopup(SelectedGroup);
            return;
        }
    }
    [RelayCommand]
    private async Task DeleteGroup(UserGroupDto userGroupDto)
    {
        if (userGroupDto == null) return;

        _logger?.LogWarning("Requesting deletion for User: {UserName} ({Id})", userGroupDto.Name, userGroupDto.Id);

        var result = MessageBox.Show(
            $"Are you sure you want to permanently delete user '{userGroupDto.Name}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _mediator.Send(new DeleteCommand<UserDto> { Id = userGroupDto.Id });
                _logger.LogInformation("User {Id} deleted successfully.", userGroupDto.Id);
                await LoadData();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {Id}", userGroupDto.Id);
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            _logger.LogInformation("Deletion cancelled by user.");
        }
    }

    public UserListDepartmentsViewModel(IMediator mediator, IDialogService dialogService,
        IViewModelFactory viewModelFactory,
        ILogger<UsersListViewModel> logger)
    {
        _mediator = mediator;
        _dialogService = dialogService;
        _viewModelFactory = viewModelFactory;
        _logger = logger;
    }

    // ==========================
    // ASYNC INITIALIZATION
    // ==========================
    public override async Task InitializeAsync()
    {
        if (_isInitialized) return;

        _logger?.LogInformation("Initializing UserGroups List...");

        WeakReferenceMessenger.Default.Register<HeaderActionMessage>(this, async (r, m) =>
        {
            switch (m.ActionType)
            {
                case "Add": await AddGroup(); break;
            }
        });

        // 2. Load Data
        await LoadData();
        _isInitialized = true;
    }

    private async Task AddGroup()
    {
        _logger?.LogInformation("Adding new User Groups...");
        await OpenUserGroupPopup(null);
    }

    private async Task LoadData()
    {
        try
        {
            _logger?.LogInformation("Fetching User Groups...");

            // 1. Fetch List from DB (Background Thread)
            var groups = await _mediator?.Send(new GetAllQuery<UserGroupDto>());

            UserGroups = new ObservableCollection<UserGroupDto>(groups);

            _logger?.LogInformation("Loaded {Count} groups.", UserGroups.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to load User Groups.");
            _dialogService?.ShowMessage("Failed to load data.", "Error");
        }
    }

    private async Task OpenUserGroupPopup(UserGroupDto? groupToEdit)
    {
        var vm = _viewModelFactory?.Create<UserGroupEditViewModel>();
        await vm.InitializeAsync(groupToEdit);
        try
        {
            _logger?.LogInformation("Showing UserGroupEditView dialog.");
            bool? result = _dialogService?.ShowDialog(vm);
            if (result == true)
            {
                _logger?.LogInformation("Dialog returned True (Saved). Reloading list.");
                await LoadData();
            }
            else
            {
                _logger?.LogInformation("Dialog returned False or was closed (Cancelled). No action taken.");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error opening User Group Popup.");
            _dialogService?.ShowMessage("Could not open editor.", "Error");
        }

    }
}