using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MES.Presentation.UI.Modules.UserManagement.Models
{
    /// <summary>
    /// Represents a node in the User Group Rights tree.
    /// Leaf nodes (IsLeaf = true) have a ScreenKey and a CanAssign flag.
    /// Parent nodes aggregate children states.
    /// </summary>
    public partial class ScreenRightNode : ObservableObject
    {
        [ObservableProperty]
        private string _displayName = string.Empty;

        [ObservableProperty]
        private string? _screenKey;

        [ObservableProperty]
        private bool _isAssigned;

        public bool IsLeaf => Children.Count == 0;

        public ObservableCollection<ScreenRightNode> Children { get; } = new();

        partial void OnIsAssignedChanged(bool value)
        {
            // Propagate to children when a parent node is toggled
            foreach (var child in Children)
                child.IsAssigned = value;
        }
    }
}
