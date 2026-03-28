using MES.Presentation.UI.Bootstrapper;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Your extension method
            services.AddPresentationUI();

            _serviceProvider = services.BuildServiceProvider();


            var mainWindow = new MainWindowView
            {
                DataContext = _serviceProvider
                    .GetRequiredService<MainWindowViewModel>()
            };

            mainWindow.Show();
        }
    }

}
