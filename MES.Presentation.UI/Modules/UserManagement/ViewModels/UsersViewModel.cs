using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging; // Required for Messenger
using MediatR; // Required for passing to children
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Controls.ListHeaderBar;
using MES.Presentation.UI.Messages;
using MES.Presentation.UI.Service;

namespace MES.Presentation.UI.Modules.UserManagement.ViewModels
{
    public partial class UsersViewModel : BaseViewModel 
    {
        // We need to store this to pass it to the children (List & Departments)
        private readonly IMediator _mediator;
        private readonly IDialogService _dialogService;
        private readonly IViewModelFactory _viewModelFactory;

        public ListHeaderBarViewModel<UsersTab>? Header { get; set; }

        [ObservableProperty]
        private BaseViewModel? currentContentViewModel;

        // Inject Mediator here
        public UsersViewModel(IMediator mediator,
                    IDialogService dialogService,
                   IViewModelFactory viewModelFactory)
        {
            _mediator = mediator;
            _dialogService = dialogService;
            _viewModelFactory = viewModelFactory;
        }

        public override void Initialize()
        {
            Header = new ListHeaderBarViewModel<UsersTab>
            {
                CanAdd = true,
                CanEdit = true,
                CanDelete = false,
                CanRefresh = true,
                CanExport = true,
                CanCopy = true,

                AddCommand = new RelayCommand(() => SendHeaderMessage("Add")),
                EditCommand = new RelayCommand(() => SendHeaderMessage("Edit")),
                RefreshCommand = new RelayCommand(() => SendHeaderMessage("Refresh")),
                DeleteCommand = new RelayCommand(() => SendHeaderMessage("Delete"))
            };

            Header.Tabs.Add(UsersTab.Users);
            Header.Tabs.Add(UsersTab.UserDepartments);

            Header.SelectedTab = UsersTab.Users;
            Header.TabChangedRequested += OnTabChanged;

            // Initialize default view
            // Pass the mediator to the child
            CurrentContentViewModel = _viewModelFactory.Create<UsersListViewModel>();
        }

        public override async Task InitializeAsync()
        {
            var usersList = _viewModelFactory.Create<UsersListViewModel>();
            Header = new ListHeaderBarViewModel<UsersTab>
            {
                CanAdd = true,
                CanEdit = true,
                CanDelete = false,
                CanRefresh = true,
                CanExport = true,
                CanCopy = true,

                AddCommand = new RelayCommand(() => SendHeaderMessage("Add")),
                EditCommand = new RelayCommand(() => SendHeaderMessage("Edit")),
                RefreshCommand = new RelayCommand(() => SendHeaderMessage("Refresh")),
                DeleteCommand = new RelayCommand(() => SendHeaderMessage("Delete"))
            };

            Header.Tabs.Add(UsersTab.Users);
            Header.Tabs.Add(UsersTab.UserDepartments);

            Header.SelectedTab = UsersTab.Users;
            Header.TabChangedRequested += OnTabChanged;

            // Initialize default view
            // Pass the mediator to the child
            CurrentContentViewModel = usersList;
            CurrentContentViewModel?.InitializeAsync();
        }

        // Helper to broadcast the button click
        private void SendHeaderMessage(string action)
        {
            WeakReferenceMessenger.Default.Send(new HeaderActionMessage(action));
        }

        private void OnTabChanged(UsersTab tab)
        {
            if (CurrentContentViewModel != null)
            {
                CurrentContentViewModel.Cleanup();
            }

            switch (tab)
            {
                case UsersTab.Users:
                    CurrentContentViewModel = _viewModelFactory.Create<UsersListViewModel>();
                    break;

                case UsersTab.UserDepartments:
                    CurrentContentViewModel = _viewModelFactory.Create<UserListDepartmentsViewModel>();
                    break;
            }

            CurrentContentViewModel?.InitializeAsync();
        }
    }
}