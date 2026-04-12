using MES.Presentation.UI.Bootstrapper;
using MES.Presentation.UI.Views;
using MES.Presentation.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using MES.Infrastructure.Data;

namespace MES.Presentation.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // Your extension method
        services.AddPresentationUI();

        _serviceProvider = services.BuildServiceProvider();

#if DEBUG
        var dbContext = _serviceProvider.GetService<MesDbContext>();
        dbContext?.Database.EnsureCreated();
#endif
        var mainWindow = new Shell
        {
            DataContext = _serviceProvider
                .GetRequiredService<ShellViewModel>()
        };

        mainWindow.Show();
    }
}