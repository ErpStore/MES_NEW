using MES.Presentation.UI.Bootstrapper;
using MES.Presentation.UI.Controls;
using MES.Presentation.UI.Modules.UserManagement.ViewModels;
using MES.Presentation.UI.Navigation;
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

            // Set shutdown mode to explicit to prevent auto-shutdown
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try
            {
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

                // Create and configure main window
                var mainWindowViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
                mainWindowViewModel.Initialize();

                var mainWindow = new MainWindowView
                {
                    DataContext = mainWindowViewModel
                };

                // Set as application's main window BEFORE showing it
                MainWindow = mainWindow;

                // Navigate to Users page after setting main window
                var shell = _serviceProvider.GetRequiredService<ShellViewModel>();
                shell.NavigateTo(AppPage.Users);

                // Change shutdown mode back to normal behavior
                ShutdownMode = ShutdownMode.OnMainWindowClose;

                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application startup failed: {ex.Message}", "Startup Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }


    }

}
