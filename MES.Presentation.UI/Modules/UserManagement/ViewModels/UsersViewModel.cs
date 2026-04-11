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
        private readonly ICurrentUserService _currentUserService;

        public ListHeaderBarViewModel<UsersTab>? Header { get; set; }

        [ObservableProperty]
        private BaseViewModel? currentContentViewModel;

        public UsersViewModel(IMediator mediator,
                    IDialogService dialogService,
                   IViewModelFactory viewModelFactory,
                   ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _dialogService = dialogService;
            _viewModelFactory = viewModelFactory;
            _currentUserService = currentUserService;
        }

        public override void Initialize()
        {
            var usersRights = _currentUserService.GetRights(ScreenKeys.Users);

            Header = new ListHeaderBarViewModel<UsersTab>
            {
                CanAdd = usersRights?.CanAdd ?? true,
                CanEdit = usersRights?.CanEdit ?? true,
                CanDelete = usersRights?.CanDelete ?? false,
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
            Header.Tabs.Add(UsersTab.UserRights);

            Header.SelectedTab = UsersTab.Users;
            Header.TabChangedRequested += OnTabChanged;

            CurrentContentViewModel = _viewModelFactory.Create<UsersListViewModel>();
        }

        public override async Task InitializeAsync()
        {
            var usersList = _viewModelFactory.Create<UsersListViewModel>();
            var usersRights = _currentUserService.GetRights(ScreenKeys.Users);

            Header = new ListHeaderBarViewModel<UsersTab>
            {
                CanAdd = usersRights?.CanAdd ?? true,
                CanEdit = usersRights?.CanEdit ?? true,
                CanDelete = usersRights?.CanDelete ?? false,
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
            Header.Tabs.Add(UsersTab.UserRights);

            Header.SelectedTab = UsersTab.Users;
            Header.TabChangedRequested += OnTabChanged;

            CurrentContentViewModel = usersList;
            CurrentContentViewModel?.InitializeAsync();
        }

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

                case UsersTab.UserRights:
                    CurrentContentViewModel = _viewModelFactory.Create<UserRightsViewModel>();
                    break;
            }

            CurrentContentViewModel?.InitializeAsync();
        }
    }
}
