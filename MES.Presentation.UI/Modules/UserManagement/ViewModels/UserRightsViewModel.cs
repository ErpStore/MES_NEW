using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.User.Commands;
using MES.ApplicationLayer.User.Dtos;
using MES.ApplicationLayer.User.Quires;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Modules.UserManagement.Models;
using MES.Presentation.UI.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace MES.Presentation.UI.Modules.UserManagement.ViewModels;

public partial class UserRightsViewModel : BaseViewModel
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    private readonly ILogger<UserRightsViewModel>? _logger;

    public ObservableCollection<ScreenRightNode> RightNodes { get; } = new();
    public ObservableCollection<UserDto> Users { get; } = new();

    [ObservableProperty]
    private UserDto? _selectedUser;

    [ObservableProperty]
    private bool _isAdmin;

    public UserRightsViewModel(ICurrentUserService currentUserService, IMediator mediator, ILogger<UserRightsViewModel>? logger = null)
    {
        _currentUserService = currentUserService;
        _mediator = mediator;
        _logger = logger;
    }

    public override void Initialize()
    {
        IsAdmin = _currentUserService.IsAdmin;
    }

    public override async Task InitializeAsync()
    {
        IsAdmin = _currentUserService.IsAdmin;

        if (IsAdmin)
        {
            await LoadUsers();
        }
        else
        {
            // Non-admin: show current user's own rights (read-only view)
            BuildRightsTree(null);
        }
    }

    partial void OnSelectedUserChanged(UserDto? value)
    {
        if (value != null)
            _ = LoadRightsForUserAsync(value.Id);
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
            _logger?.LogError(ex, "Failed to load users.");
        }
    }

    private async Task LoadRightsForUserAsync(int userId)
    {
        try
        {
            var userRights = await _mediator.Send(new GetUserRightsQuery { UserId = userId });
            BuildRightsTree(userRights);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to load rights for user {UserId}.", userId);
        }
    }

    private void BuildRightsTree(List<UserRightDto>? userRights)
    {
        RightNodes.Clear();

        ScreenRightNode MakeLeaf(string displayName, string screenKey)
        {
            if (userRights != null)
            {
                // Admin editing another user: show per-user rights if they exist, otherwise all unchecked
                var right = userRights.FirstOrDefault(r =>
                    string.Equals(r.ScreenKey, screenKey, StringComparison.OrdinalIgnoreCase));
                return new ScreenRightNode
                {
                    DisplayName = displayName,
                    ScreenKey = screenKey,
                    CanAdd = right?.CanAdd ?? false,
                    CanEdit = right?.CanEdit ?? false,
                    CanDelete = right?.CanDelete ?? false
                };
            }
            else
            {
                // Non-admin: show current user's effective rights (read-only)
                var right = _currentUserService.GetRights(screenKey);
                return new ScreenRightNode
                {
                    DisplayName = displayName,
                    ScreenKey = screenKey,
                    CanAdd = right?.CanAdd ?? false,
                    CanEdit = right?.CanEdit ?? false,
                    CanDelete = right?.CanDelete ?? false
                };
            }
        }

        var root = new ScreenRightNode { DisplayName = "#1 EnJoin MES-Manual chemical System" };

        // #1.0 Overview
        var overview = new ScreenRightNode { DisplayName = "#1.0 OverView" };
        var overviewSub = new ScreenRightNode { DisplayName = "#1.0.0 OverView" };
        overviewSub.Children.Add(MakeLeaf("#1.0.0.1 OverView", ScreenKeys.Overview));
        overview.Children.Add(overviewSub);
        root.Children.Add(overview);

        // #1.1 Configuration
        var config = new ScreenRightNode { DisplayName = "#1.1 Configuration" };
        var desc = new ScreenRightNode { DisplayName = "#1.1.1 Description" };
        desc.Children.Add(MakeLeaf("#1.1.1.1 Plant Description", "PlantDescription"));
        config.Children.Add(desc);

        var userAdmin = new ScreenRightNode { DisplayName = "#1.1.2 User Administration" };
        userAdmin.Children.Add(MakeLeaf("#1.1.2.1 Users", ScreenKeys.Users));
        userAdmin.Children.Add(MakeLeaf("#1.1.2.2 User Group", ScreenKeys.UserGroups));
        config.Children.Add(userAdmin);
        root.Children.Add(config);

        // #1.2 Material
        var material = new ScreenRightNode { DisplayName = "#1.2 Material" };
        var materialSub = new ScreenRightNode { DisplayName = "#1.2.1 Material" };
        materialSub.Children.Add(MakeLeaf("#1.2.1.1 Material Group", ScreenKeys.MaterialGroup));
        materialSub.Children.Add(MakeLeaf("#1.2.1.2 Material Management", ScreenKeys.MaterialManagement));
        materialSub.Children.Add(MakeLeaf("#1.2.1.3 Feeding Path", ScreenKeys.FeedingPath));
        material.Children.Add(materialSub);
        root.Children.Add(material);

        // #1.3 Recipe
        var recipe = new ScreenRightNode { DisplayName = "#1.3 Recipe" };
        var recipeSub = new ScreenRightNode { DisplayName = "#1.3.1 Recipes" };
        recipeSub.Children.Add(MakeLeaf("#1.3.1.1 Recipe Management", ScreenKeys.RecipeManagement));
        recipeSub.Children.Add(MakeLeaf("#1.3.1.2 Recipe Process", ScreenKeys.RecipeProcess));
        recipe.Children.Add(recipeSub);
        root.Children.Add(recipe);

        // #1.4 Order
        var order = new ScreenRightNode { DisplayName = "#1.4 Order" };
        var orderSub = new ScreenRightNode { DisplayName = "#1.4.1 Order" };
        orderSub.Children.Add(MakeLeaf("#1.4.1.1 Order Management", ScreenKeys.OrderManagement));
        order.Children.Add(orderSub);
        root.Children.Add(order);

        RightNodes.Add(root);
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        if (SelectedUser == null) return;

        try
        {
            var rights = CollectLeafRights(SelectedUser.Id);
            await _mediator.Send(new SaveUserRightsCommand
            {
                UserId = SelectedUser.Id,
                Rights = rights
            });

            _logger?.LogInformation("Saved {Count} rights for UserId {Id}", rights.Count, SelectedUser.Id);
            System.Windows.MessageBox.Show("Rights saved successfully.", "Saved", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to save rights.");
            System.Windows.MessageBox.Show($"Failed to save user rights: {ex.Message}", "Error");
        }
    }

    private bool CanSave() => IsAdmin && SelectedUser != null;

    private List<UserRightDto> CollectLeafRights(int userId)
    {
        var result = new List<UserRightDto>();
        CollectFromNode(RightNodes, userId, result);
        return result;
    }

    private static void CollectFromNode(IEnumerable<ScreenRightNode> nodes, int userId, List<UserRightDto> result)
    {
        foreach (var node in nodes)
        {
            if (node.IsLeaf && node.ScreenKey != null)
            {
                result.Add(new UserRightDto
                {
                    UserId = userId,
                    ScreenKey = node.ScreenKey,
                    CanAdd = node.CanAdd,
                    CanEdit = node.CanEdit,
                    CanDelete = node.CanDelete
                });
            }
            else
            {
                CollectFromNode(node.Children, userId, result);
            }
        }
    }
}