using MediatR;
using MES.ApplicationLayer.User.Dtos;

namespace MES.ApplicationLayer.User.Commands
{
    public class SaveUserGroupRightsCommand : IRequest<bool>
    {
        public int UserGroupId { get; set; }
        public required List<UserGroupRightDto> Rights { get; set; }
    }
}
