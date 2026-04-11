using MES.Presentation.UI.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MES.Presentation.UI.Service
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        // =============================================
        // 1. VIEW REGISTRY (Maps ViewModel -> Window)
        // =============================================
        private static readonly Dictionary<Type, Type> _mappings = new();

        public DialogService(IServiceProvider serviceProvider, ILogger<DialogService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Call this in your Startup/DependencyInjection to map ViewModels to Views.
        /// Example: DialogService.RegisterDialog<UserEditViewModel, UserEditView>();
        /// </summary>
        public static void RegisterDialog<TViewModel, TView>()
            where TView : Window
        {
            _mappings[typeof(TViewModel)] = typeof(TView);
        }

        // =============================================
        // 2. SHOW DIALOG (The Generic Logic)
        // =============================================
        public bool? ShowDialog<TViewModel>(TViewModel viewModel)
        {
            var vmType = typeof(TViewModel);
            _logger.LogInformation("Request to show dialog for ViewModel: {ViewModelType}", vmType.Name);

            // Check if we know which Window to open
            if (!_mappings.ContainsKey(vmType))
            {
                var error = $"No View registered for ViewModel: {vmType.Name}";
                _logger.LogCritical(error);
                throw new InvalidOperationException($"No View registered for ViewModel: {vmType.Name}. Did you forget to call DialogService.RegisterDialog?");
            }

            try
            {
                // 1. Create the Window Instance dynamically
                var windowType = _mappings[vmType];
                _logger.LogDebug("Creating window of type: {WindowType}", windowType.Name);

                var window = (Window)Activator.CreateInstance(windowType);

                if(window == null)
                {
                    var error = $"Could not create instance of Window: {windowType.Name}";
                    _logger.LogCritical(error);
                    throw new InvalidOperationException(error);
                }

                // 2. Configure the Window
                window.DataContext = viewModel;
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null && mainWindow != window)
                {
                    window.Owner = mainWindow;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                else
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }

                // We check if the ViewModel implements our Interface
                if (viewModel is IModalViewModel modalVm)
                {
                    modalVm.CloseAction = (result) =>
                    {
                        // This creates the bridge: ViewModel calls CloseAction -> Window Closes
                        try
                        {
                            _logger.LogDebug("ViewModel requested close with result: {Result}", result);
                            window.DialogResult = result;
                        }
                        catch (InvalidOperationException)
                        {
                            _logger.LogWarning("Window already closed.");
                            window.Close();
                        }
                    };
                }

                _logger.LogInformation("Showing Dialog...");
                return window.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing dialog for ViewModel: {ViewModelType}", vmType.Name);
                throw;
            }
        }

        // =============================================
        // 3. HELPER POPUPS
        // =============================================
        public void ShowMessage(string message, string title)
        {
            _logger.LogInformation("Showing Message Box: [{Title}] {Message}", title, message);
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ShowConfirmation(string message, string title)
        {
            _logger.LogInformation("Showing Confirmation Box: [{Title}] {Message}", title, message);
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);

            bool confirmed = result == MessageBoxResult.Yes;
            _logger.LogInformation("User response: {Confirmed}", confirmed);

            return confirmed;
        }
    }
}

