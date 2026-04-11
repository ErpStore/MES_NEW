using MES.ApplicationLayer.User.Dtos;

namespace MES.Presentation.UI.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private UserDto? _currentUser;
        private List<UserGroupRightDto> _groupRights = new();
        private List<UserRightDto> _userRights = new();

        public bool IsLoggedIn => _currentUser != null;

        public bool IsAdmin => _currentUser?.Id == 1;

        public UserDto? CurrentUser => _currentUser;

        public void Login(UserDto user, List<UserGroupRightDto> groupRights, List<UserRightDto> userRights)
        {
            _currentUser = user;
            _groupRights = groupRights ?? new List<UserGroupRightDto>();
            _userRights = userRights ?? new List<UserRightDto>();
        }

        public void Logout()
        {
            _currentUser = null;
            _groupRights.Clear();
            _userRights.Clear();
        }

        public UserGroupRightDto? GetRights(string screenKey)
        {
            // Per-user rights take priority over user-group rights
            var userRight = _userRights.FirstOrDefault(r =>
                string.Equals(r.ScreenKey, screenKey, StringComparison.OrdinalIgnoreCase));

            if (userRight != null)
            {
                return new UserGroupRightDto
                {
                    ScreenKey = userRight.ScreenKey,
                    CanAdd = userRight.CanAdd,
                    CanEdit = userRight.CanEdit,
                    CanDelete = userRight.CanDelete
                };
            }

            return _groupRights.FirstOrDefault(r =>
                string.Equals(r.ScreenKey, screenKey, StringComparison.OrdinalIgnoreCase));
        }
    }
}
