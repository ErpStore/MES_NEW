using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Orders.DTO;
using MES.Domain.Entities.Orders;
using MES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MES.ApplicationLayer.Orders.Handler
{
    public class ProductionOrderHandler :
         IRequestHandler<GetAllQuery<ProductionOrderDto>, List<ProductionOrderDto>>,
         IRequestHandler<SaveCommand<ProductionOrderDto>, int>,
         IRequestHandler<DeleteCommand<ProductionOrderDto>, int>
    {
        private readonly MesDbContext _context;

        public ProductionOrderHandler(MesDbContext context) { _context = context; }

        public async Task<List<ProductionOrderDto>> Handle(GetAllQuery<ProductionOrderDto> request, CancellationToken token)
        {
            return await _context.ProductionOrder.AsNoTracking()
                .Include(o => o.Recipe) // Join to get Recipe details
                .OrderBy(o => o.SerialNumber)
                .Select(x => new ProductionOrderDto
                {
                    Id = x.Id,
                    SerialNumber = x.SerialNumber,
                    RecipeId = x.RecipeId,
                    RecipeName = x.Recipe.RecipeName,
                    RecipeCode = x.Recipe.RecipeCode,
                    SetBatch = x.SetBatch,
                    ActualBatch = x.ActualBatch,
                    Status = x.Status,
                    IsReleased = x.IsReleased,
                    BatchStart = x.BatchStart,
                    BatchEnd = x.BatchEnd,
                    Description = x.Description
                })
                .ToListAsync(token);
        }

        public async Task<int> Handle(SaveCommand<ProductionOrderDto> request, CancellationToken token)
        {
            var dto = request.Data;
            ProductionOrder entity;

            if (dto.Id == 0)
            {
                entity = new ProductionOrder(); 
                _context.ProductionOrder.Add(entity);
            }
            else
            {
                entity = await _context.ProductionOrder.FindAsync(new object[] { dto.Id }, token);
            }

            entity.SerialNumber = dto.SerialNumber;
            entity.RecipeId = dto.RecipeId;
            entity.SetBatch = dto.SetBatch;
            entity.ActualBatch = dto.ActualBatch;
            entity.Status = dto.Status;
            entity.IsReleased = dto.IsReleased;
            entity.BatchStart = dto.BatchStart;
            entity.BatchEnd = dto.BatchEnd;
            entity.Description = dto.Description;

            await _context.SaveChangesAsync(token);
            return entity.Id;
        }

        public async Task<int> Handle(DeleteCommand<ProductionOrderDto> request, CancellationToken token)
        {
            var entity = await _context.ProductionOrder.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.ProductionOrder.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
            return request.Id;
        }
    }
}
