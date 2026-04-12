using CommunityToolkit.Mvvm.ComponentModel;
using MES.Presentation.UI.Common;
using System.Collections.ObjectModel;
using System.Windows.Input; // Required for ICommand

namespace MES.Presentation.UI.Controls.ListHeaderBar;

// Generics <T> allows this header to be used for Users, Orders, Recipes, etc.
public partial class ListHeaderBarViewModel<T> : BaseViewModel
{
    // ============================
    // 1. VISIBILITY FLAGS (State)
    // ============================
    [ObservableProperty] private bool _canAdd;
    [ObservableProperty] private bool _canEdit;
    [ObservableProperty] private bool _canDelete;
    [ObservableProperty] private bool _canRefresh;
    [ObservableProperty] private bool _canCopy;
    [ObservableProperty] private bool _canPaste;
    [ObservableProperty] private bool _canExport;
    [ObservableProperty] private bool _canPrint;

    // ============================
    // 2. COMMANDS (Assigned by Parent)
    // ============================
    // We use ICommand properties so the Parent (UsersViewModel) can inject the logic
    public ICommand? AddCommand { get; set; }
    public ICommand? EditCommand { get; set; }
    public ICommand? DeleteCommand { get; set; }
    public ICommand? RefreshCommand { get; set; }
    public ICommand? CopyCommand { get; set; }
    public ICommand? PasteCommand { get; set; }
    public ICommand? ExportCommand { get; set; }
    public ICommand? PrintCommand { get; set; }

    // ============================
    // 3. TAB NAVIGATION
    // ============================
    public ObservableCollection<T> Tabs { get; } = new();

    [ObservableProperty]
    private T _selectedTab;

    // Event to notify Parent when tab changes
    public event Action<T>? TabChangedRequested;

    partial void OnSelectedTabChanged(T value)
    {
        TabChangedRequested?.Invoke(value);
    }

    // ============================
    // 4. SEARCH BOX
    // ============================
    [ObservableProperty]
    private string _searchText;
}