using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MES.Presentation.UI.Common;

public class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ViewModelFactory>? _logger;

    public ViewModelFactory(IServiceProvider serviceProvider, ILogger<ViewModelFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public T Create<T>() where T : BaseViewModel
    {
        _logger.LogDebug("Factory creating instance of {ViewModelType}", typeof(T).Name);

        try
        {
            return _serviceProvider.GetRequiredService<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Factory failed to resolve {ViewModelType}. Check DependencyInjection.cs.", typeof(T).Name);
            throw;
        }
    }
}