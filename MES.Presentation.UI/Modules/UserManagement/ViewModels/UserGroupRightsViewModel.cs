using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MES.ApplicationLayer.User.Commands;
using MES.ApplicationLayer.User.Dtos;
using MES.ApplicationLayer.User.Quires;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Modules.UserManagement.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace MES.Presentation.UI.Modules.UserManagement.ViewModels
{
    public partial class UserGroupRightsViewModel : BaseViewModel
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserGroupRightsViewModel>? _logger;

        private int _userGroupId;

        [ObservableProperty]
        private string _windowTitle = "User Group Rights";

        [ObservableProperty]
        private string _searchText = string.Empty;

        public ObservableCollection<ScreenRightNode> RightNodes { get; } = new();

        public UserGroupRightsViewModel(IMediator mediator, ILogger<UserGroupRightsViewModel>? logger = null)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task InitializeAsync(UserGroupDto userGroupDto)
        {
            _userGroupId = userGroupDto.Id;
            WindowTitle = $"User Group Rights \u2013 {userGroupDto.Name}";

            var existingRights = await _mediator.Send(new GetUserGroupRightsQuery { UserGroupId = _userGroupId });

            BuildRightsTree(existingRights);
        }

        private void BuildRightsTree(List<UserGroupRightDto> existingRights)
        {
            RightNodes.Clear();

            bool IsAssigned(string key) =>
                existingRights.Any(r => string.Equals(r.ScreenKey, key, StringComparison.OrdinalIgnoreCase));

            var root = new ScreenRightNode { DisplayName = "EnJoin MES-Manual chemical System" };

            // #1.0 Overview
            var overview = new ScreenRightNode { DisplayName = "#1.0 OverView" };
            var overviewSub = new ScreenRightNode { DisplayName = "#1.0.0 OverView" };
            overviewSub.Children.Add(new ScreenRightNode
            {
                DisplayName = "#1.0.0.0.1 OverView",
                ScreenKey = ScreenKeys.Overview,
                IsAssigned = IsAssigned(ScreenKeys.Overview)
            });
            overview.Children.Add(overviewSub);
            root.Children.Add(overview);

            // #1.1 Configuration
            var config = new ScreenRightNode { DisplayName = "#1.1 Configuration" };
            var desc = new ScreenRightNode { DisplayName = "#1.1.1 Description" };
            desc.Children.Add(new ScreenRightNode
            {
                DisplayName = "#1.1.1.1 Plant Description",
                ScreenKey = "PlantDescription",
                IsAssigned = IsAssigned("PlantDescription")
            });
            config.Children.Add(desc);

            var userAdmin = new ScreenRightNode { DisplayName = "#1.1.2 User Administration" };
            userAdmin.Children.Add(new ScreenRightNode
            {
                DisplayName = "#1.1.2.1 Users",
                ScreenKey = ScreenKeys.Users,
                IsAssigned = IsAssigned(ScreenKeys.Users)
            });
            userAdmin.Children.Add(new ScreenRightNode
            {
                DisplayName = "#1.1.2.2 User Group",
                ScreenKey = ScreenKeys.UserGroups,
                IsAssigned = IsAssigned(ScreenKeys.UserGroups)
            });
            config.Children.Add(userAdmin);
            root.Children.Add(config);

            // #1.2 Material
            var material = new ScreenRightNode { DisplayName = "#1.2 Material" };
            var materialSub = new ScreenRightNode { DisplayName = "#1.2.1 Material" };
            materialSub.Children.Add(new ScreenRightNode
            {
                DisplayName = "#1.2.1.1 Material Group",
                ScreenKey = ScreenKeys.MaterialGroup,
                IsAssigned = IsAssigned(ScreenKeys.MaterialGroup)
            });
            materialSub.Children.Add(new ScreenRightNode
            {
                DisplayName = "#1.2.1.2 Material Management",
                ScreenKey = ScreenKeys.MaterialManagement,
                IsAssigned = IsAssigned(ScreenKeys.MaterialManagement)
            });
            materialSub.Children.Add(new ScreenRightNode
            {
                DisplayName = "#1.2.1.3 Feeding Path",
                ScreenKey = ScreenKeys.FeedingPath,
                IsAssigned = IsAssigned(ScreenKeys.FeedingPath)
            });
            material.Children.Add(materialSub);
            root.Children.Add(material);

            // #1.3 Recipe
            var recipe = new ScreenRightNode { DisplayName = "#1.3 Recipe" };
            var recipeSub = new ScreenRightNode { DisplayName = "#1.3.1 Recipes" };
            recipeSub.Children.Add(new ScreenRightNode
            {
                DisplayName = "#1.3.1.1 Recipe Management",
                ScreenKey = ScreenKeys.RecipeManagement,
                IsAssigned = IsAssigned(ScreenKeys.RecipeManagement)
            });
            recipeSub.Children.Add(new ScreenRightNode
            {
                DisplayName = "#1.3.1.2 Recipe Process",
                ScreenKey = ScreenKeys.RecipeProcess,
                IsAssigned = IsAssigned(ScreenKeys.RecipeProcess)
            });
            recipe.Children.Add(recipeSub);
            root.Children.Add(recipe);

            // #1.4 Order
            var order = new ScreenRightNode { DisplayName = "#1.4 Order" };
            var orderSub = new ScreenRightNode { DisplayName = "#1.4.1 Order" };
            orderSub.Children.Add(new ScreenRightNode
            {
                DisplayName = "#1.4.1.1 Order Management",
                ScreenKey = ScreenKeys.OrderManagement,
                IsAssigned = IsAssigned(ScreenKeys.OrderManagement)
            });
            order.Children.Add(orderSub);
            root.Children.Add(order);

            RightNodes.Add(root);
        }

        [RelayCommand]
        private async Task Accept()
        {
            try
            {
                var rights = CollectLeafRights();

                await _mediator.Send(new SaveUserGroupRightsCommand
                {
                    UserGroupId = _userGroupId,
                    Rights = rights
                });

                _logger?.LogInformation("Saved {Count} rights for UserGroupId {Id}", rights.Count, _userGroupId);
                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to save rights.");
                System.Windows.MessageBox.Show($"Error saving rights: {ex.Message}", "Error");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }

        private List<UserGroupRightDto> CollectLeafRights()
        {
            var result = new List<UserGroupRightDto>();
            CollectFromNode(RightNodes, _userGroupId, result);
            return result;
        }

        private static void CollectFromNode(IEnumerable<ScreenRightNode> nodes, int userGroupId, List<UserGroupRightDto> result)
        {
            foreach (var node in nodes)
            {
                if (node.IsLeaf && node.ScreenKey != null)
                {
                    result.Add(new UserGroupRightDto
                    {
                        UserGroupId = userGroupId,
                        ScreenKey = node.ScreenKey,
                        CanAdd = node.IsAssigned,
                        CanEdit = node.IsAssigned,
                        CanDelete = node.IsAssigned
                    });
                }
                else
                {
                    CollectFromNode(node.Children, userGroupId, result);
                }
            }
        }
    }
}
