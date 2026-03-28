using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace MES.Presentation.UI.Common
{
    public abstract partial class BaseViewModel : ObservableValidator, IModalViewModel
    {
        [ObservableProperty]
        private bool? _isBusy;

        [ObservableProperty]
        private string? _title;

        public Action<bool>? CloseAction { get; set; }

        public virtual async Task InitializeAsync() { }

        public virtual void Initialize() { }


        // Every ViewModel knows how to clean itself up.
        public virtual void Cleanup()
        {
            // By default, stop listening to all messages.
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
