using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Recipes.Dtos;
using MES.Domain.Entities.Recipes;
using MES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MES.ApplicationLayer.Recipes.Handler
{
    public class RecipeHandler :
        IRequestHandler<GetAllQuery<RecipeDto>, List<RecipeDto>>,
        IRequestHandler<SaveCommand<RecipeDto>, int>,
        IRequestHandler<DeleteCommand<RecipeDto>, int>
    {
        private readonly MesDbContext _context;

        public RecipeHandler(MesDbContext context) { _context = context; }

        public async Task<List<RecipeDto>> Handle(GetAllQuery<RecipeDto> request, CancellationToken token)
        {
            return await _context.Recipes.AsNoTracking()
                .Select(x => new RecipeDto
                {
                    Id = x.Id,
                    RecipeName = x.RecipeName,
                    RecipeCode = x.RecipeCode,
                    Version = x.Version,
                    Type = x.Type,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    IsBlocked = x.IsBlocked,
                    BatchWeight = x.BatchWeight,
                    Description = x.Description
                })
                .ToListAsync(token);
        }

        public async Task<int> Handle(SaveCommand<RecipeDto> request, CancellationToken token)
        {
            var dto = request.Data;
            Recipe entity;

            if (dto.Id == 0)
            {
                entity = new Recipe { CreatedDate = DateTime.Now };
                _context.Recipes.Add(entity);
            }
            else
            {
                entity = await _context.Recipes.FindAsync(new object[] { dto.Id }, token);
                if (entity == null) throw new KeyNotFoundException("Recipe not found");
            }

            entity.RecipeName = dto.RecipeName;
            entity.RecipeCode = dto.RecipeCode;
            entity.Version = dto.Version;
            entity.Type = dto.Type;
            entity.ModifiedDate = DateTime.Now;
            entity.IsBlocked = dto.IsBlocked;
            entity.BatchWeight = dto.BatchWeight;
            entity.Description = dto.Description;

            await _context.SaveChangesAsync(token);
            return entity.Id;
        }

        public async Task<int> Handle(DeleteCommand<RecipeDto> request, CancellationToken token)
        {
            var entity = await _context.Recipes.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.Recipes.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
            return request.Id;
        }
    }
}
