using CommunityToolkit.Mvvm.ComponentModel;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Controls;

namespace MES.Presentation.UI.Shell
{
    public partial class MainWindowViewModel : BaseViewModel
    {

        private const string DefaultTitle = "TRIKALA ERP";

        [ObservableProperty]
        private ShellViewModel shell;

        [ObservableProperty]
        private HeaderBarViewModel headerBar;

        [ObservableProperty]
        private SideMenuViewModel sideMenu;

        public string? ApplicationTitle;

        public MainWindowViewModel(
            ShellViewModel shell,
            HeaderBarViewModel headerBar,
            SideMenuViewModel sideMenu)
        {
            this.shell = shell;
            this.headerBar = headerBar;
            this.sideMenu = sideMenu;
        }

        public override void Initialize()
        {
            ApplicationTitle = DefaultTitle;
        }
    }
}
