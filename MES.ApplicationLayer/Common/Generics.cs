using MediatR;

namespace MES.ApplicationLayer.Common
{
    // 1. Generic GET Query
    // TResponse = The DTO you want back (e.g., UserGroupDto)
    public class GetAllQuery<TResponse> : IRequest<List<TResponse>>
    {
    }

    // 2. Generic SAVE Command
    // TDto = The object being saved. It must have an Id property.
    public class SaveCommand<TDto> : IRequest<int>
    {
        public required TDto Data { get; set; }
    }

    // 3. Generic DELETE Command
    // We use TDto just to make the type unique so MediatR finds the right handler.
    // The actual data we need is just the ID.
    public class DeleteCommand<TDto> : IRequest<int>
    {
        public int Id { get; set; }
    }
}
