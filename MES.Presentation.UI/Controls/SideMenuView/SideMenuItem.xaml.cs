using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace MES.Presentation.UI.Controls.SideMenuView;

/// <summary>
/// Interaction logic for SideMenuItem.xaml
/// </summary>
public partial class SideMenuItem : UserControl
{
    public SideMenuItem()
    {
        InitializeComponent();
    }

    // ===== ICON =====
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string),
            typeof(SideMenuItem));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    // ===== TEXT =====
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string),
            typeof(SideMenuItem));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    // ===== COMMAND =====
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand),
            typeof(SideMenuItem));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    // ===== COMMAND PARAMETER =====
    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(nameof(CommandParameter), typeof(object),
            typeof(SideMenuItem));

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    // ===== ACTIVE STATE =====
    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(nameof(IsActive), typeof(bool),
            typeof(SideMenuItem));

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    // ===== EXPANDED STATE =====
    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register(nameof(IsExpanded), typeof(bool),
            typeof(SideMenuItem));

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public static readonly RoutedEvent ClickedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(Clicked),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SideMenuItem));


    public event RoutedEventHandler Clicked
    {
        add => AddHandler(ClickedEvent, value);
        remove => RemoveHandler(ClickedEvent, value);
    }

    private void OnClick(object sender, MouseButtonEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickedEvent, this));

        // 🔹 Execute MVVM command
        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }

}