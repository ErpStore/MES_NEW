using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Presentation.UI.Common;
using System.Windows;
using System.Windows.Threading;

namespace MES.Presentation.UI.Controls
{
    public partial class HeaderBarViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _currentDateTime;

        [ObservableProperty]
        private string _userName = "Trikala"; // Default as per image

        private readonly DispatcherTimer _timer;

        public HeaderBarViewModel()
        {
            // Initialize the timer for the real-time clock
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (s, e) => UpdateTime();
            _timer.Start();

            UpdateTime(); // Initial call
        }

        private void UpdateTime()
        {
            // Matches the format: 1/7/2026 12:39:54 AM
            CurrentDateTime = DateTime.Now.ToString("dd/M/yyyy h:mm:ss tt");
        }
    }
}
