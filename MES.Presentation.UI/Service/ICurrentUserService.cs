using MES.ApplicationLayer.User.Dtos;

namespace MES.Presentation.UI.Service
{
    public interface ICurrentUserService
    {
        bool IsLoggedIn { get; }
        bool IsAdmin { get; }
        UserDto? CurrentUser { get; }
        void Login(UserDto user, List<UserGroupRightDto> groupRights, List<UserRightDto> userRights);
        void Logout();
        UserGroupRightDto? GetRights(string screenKey);
    }
}
