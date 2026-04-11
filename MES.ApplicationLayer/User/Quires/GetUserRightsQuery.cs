using MediatR;
using MES.ApplicationLayer.User.Dtos;

namespace MES.ApplicationLayer.User.Quires
{
    public class GetUserRightsQuery : IRequest<List<UserRightDto>>
    {
        public int UserId { get; set; }
    }
}
