using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MES.Presentation.UI.Modules.UserManagement.Models
{
    /// <summary>
    /// Represents a node in the User Group Rights tree.
    /// Leaf nodes (IsLeaf = true) have a ScreenKey and permission flags.
    /// Parent nodes propagate changes to children.
    /// </summary>
    public partial class ScreenRightNode : ObservableObject
    {
        [ObservableProperty]
        private string _displayName = string.Empty;

        [ObservableProperty]
        private string? _screenKey;

        [ObservableProperty]
        private bool _isAssigned;

        [ObservableProperty]
        private bool _canAdd;

        [ObservableProperty]
        private bool _canEdit;

        [ObservableProperty]
        private bool _canDelete;

        public bool IsLeaf => Children.Count == 0;

        public ObservableCollection<ScreenRightNode> Children { get; } = new();

        partial void OnIsAssignedChanged(bool value)
        {
            // Propagate to children when a parent node is toggled
            foreach (var child in Children)
                child.IsAssigned = value;
        }

        partial void OnCanAddChanged(bool value)
        {
            foreach (var child in Children)
                child.CanAdd = value;
        }

        partial void OnCanEditChanged(bool value)
        {
            foreach (var child in Children)
                child.CanEdit = value;
        }

        partial void OnCanDeleteChanged(bool value)
        {
            foreach (var child in Children)
                child.CanDelete = value;
        }
    }
}
