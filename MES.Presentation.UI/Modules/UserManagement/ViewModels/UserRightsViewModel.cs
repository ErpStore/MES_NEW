using MES.Presentation.UI.Common;
using MES.Presentation.UI.Modules.UserManagement.Models;
using MES.Presentation.UI.Service;
using System.Collections.ObjectModel;

namespace MES.Presentation.UI.Modules.UserManagement.ViewModels
{
    public class UserRightsViewModel : BaseViewModel
    {
        private readonly ICurrentUserService _currentUserService;

        public ObservableCollection<ScreenRightNode> RightNodes { get; } = new();

        public UserRightsViewModel(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override void Initialize()
        {
            BuildRightsTree();
        }

        public override Task InitializeAsync()
        {
            BuildRightsTree();
            return Task.CompletedTask;
        }

        private void BuildRightsTree()
        {
            RightNodes.Clear();

            ScreenRightNode MakeLeaf(string displayName, string screenKey)
            {
                var right = _currentUserService.GetRights(screenKey);
                return new ScreenRightNode
                {
                    DisplayName = displayName,
                    ScreenKey = screenKey,
                    CanAdd    = right?.CanAdd    ?? false,
                    CanEdit   = right?.CanEdit   ?? false,
                    CanDelete = right?.CanDelete ?? false
                };
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
            var desc = new ScreenRightNode { DisplayName = "#1.1.1 Decription" };
            desc.Children.Add(MakeLeaf("#1.1.1.1 Plant Decription", "PlantDescription"));
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
            var recipe = new ScreenRightNode { DisplayName = "#1.3 Recpe" };
            var recipeSub = new ScreenRightNode { DisplayName = "#1.3.1 Recpes" };
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
    }
}

