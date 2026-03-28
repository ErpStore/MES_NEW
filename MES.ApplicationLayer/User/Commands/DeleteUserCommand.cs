using MediatR;

namespace MES.ApplicationLayer.User.Commands
{
    public class DeleteUserCommand : IRequest<int>
    {
        public int Id { get; set; }
    }
}
