using MediatR;
using MES.ApplicationLayer.User.Dtos;

namespace MES.ApplicationLayer.User.Commands
{
    public class SaveUserRightsCommand : IRequest<bool>
    {
        public int UserId { get; set; }
        public required List<UserRightDto> Rights { get; set; }
    }
}
