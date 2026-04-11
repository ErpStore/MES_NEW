using MES.ApplicationLayer.User.Dtos;

namespace MES.Presentation.UI.Service
{
    public interface ICurrentUserService
    {
        bool IsLoggedIn { get; }
        UserDto? CurrentUser { get; }
        void Login(UserDto user, List<UserGroupRightDto> rights);
        void Logout();
        UserGroupRightDto? GetRights(string screenKey);
    }
}
