using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Service;
using System.Windows;
using System.Windows.Threading;

namespace MES.Presentation.UI.Controls
{
    public partial class HeaderBarViewModel : ObservableObject
    {
        private readonly ICurrentUserService _currentUserService;

        [ObservableProperty]
        private string _currentDateTime;

        [ObservableProperty]
        private string _userName = "Guest";

        private readonly DispatcherTimer _timer;

        public HeaderBarViewModel(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (s, e) => UpdateTime();
            _timer.Start();

            UpdateTime();
            UpdateUserName();
        }

        private void UpdateTime()
        {
            CurrentDateTime = DateTime.Now.ToString("dd/M/yyyy h:mm:ss tt");
        }

        public void UpdateUserName()
        {
            UserName = _currentUserService.CurrentUser?.UserName ?? "Guest";
        }
    }
}
