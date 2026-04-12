using MediatR;
using MES.ApplicationLayer.User.Dtos;
namespace MES.ApplicationLayer.User.Quires;

public class GetUsersQuery : IRequest<List<UserDto>> 
{ 
    
}