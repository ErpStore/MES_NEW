using MediatR;
using MES.ApplicationLayer.User.Dtos;

namespace MES.ApplicationLayer.User.Quires
{
    public class LoginQuery : IRequest<LoginResultDto>
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
