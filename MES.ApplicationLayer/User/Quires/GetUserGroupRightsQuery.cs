using MediatR;
using MES.ApplicationLayer.User.Dtos;

namespace MES.ApplicationLayer.User.Quires
{
    public class GetUserGroupRightsQuery : IRequest<List<UserGroupRightDto>>
    {
        public int UserGroupId { get; set; }
    }
}
