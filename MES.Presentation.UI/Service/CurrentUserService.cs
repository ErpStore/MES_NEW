using MES.ApplicationLayer.User.Dtos;

namespace MES.Presentation.UI.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private UserDto? _currentUser;
        private List<UserGroupRightDto> _rights = new();

        public bool IsLoggedIn => _currentUser != null;

        public UserDto? CurrentUser => _currentUser;

        public void Login(UserDto user, List<UserGroupRightDto> rights)
        {
            _currentUser = user;
            _rights = rights ?? new List<UserGroupRightDto>();
        }

        public void Logout()
        {
            _currentUser = null;
            _rights.Clear();
        }

        public UserGroupRightDto? GetRights(string screenKey)
        {
            return _rights.FirstOrDefault(r =>
                string.Equals(r.ScreenKey, screenKey, StringComparison.OrdinalIgnoreCase));
        }
    }
}
