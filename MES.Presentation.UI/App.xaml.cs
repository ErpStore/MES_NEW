using MES.Presentation.UI.Bootstrapper;
using MES.Presentation.UI.Controls;
using MES.Presentation.UI.Modules.UserManagement.ViewModels;
using MES.Presentation.UI.Service;
using MES.Presentation.UI.Shell;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MES.Presentation.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Your extension method
            services.AddPresentationUI();

            _serviceProvider = services.BuildServiceProvider();

            // Show Login dialog before main window
            var loginVm = _serviceProvider.GetRequiredService<LoginViewModel>();
            await loginVm.InitializeAsync();

            var dialogService = _serviceProvider.GetRequiredService<IDialogService>();
            var loginResult = dialogService.ShowDialog(loginVm);

            if (loginResult != true)
            {
                // User closed login without logging in – exit application
                Shutdown();
                return;
            }

            // Update the header bar with logged-in user name
            var headerVm = _serviceProvider.GetRequiredService<HeaderBarViewModel>();
            headerVm.UpdateUserName();

            var mainWindow = new MainWindowView
            {
                DataContext = _serviceProvider
                    .GetRequiredService<MainWindowViewModel>()
            };

            mainWindow.Show();
        }
    }

}
