using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Recipes.Dtos;
using MES.ApplicationLayer.Recipes.Queries;
using MES.Domain.Entities.Recipes;
using MES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MES.ApplicationLayer.Recipes.Handler;

public class RecipeItemHandler :
    IRequestHandler<GetRecipeItemsQuery, List<RecipeItemDto>>,
    IRequestHandler<SaveCommand<RecipeItemDto>, int>,
    IRequestHandler<DeleteCommand<RecipeItemDto>, int>
{
    private readonly MesDbContext _context;

    public RecipeItemHandler(MesDbContext context) { _context = context; }

    public async Task<List<RecipeItemDto>> Handle(GetRecipeItemsQuery request, CancellationToken token)
    {
        return await _context.RecipeItems.AsNoTracking()
            .Include(ri => ri.Material) // JOIN to get Material Code for the grid
            .Where(ri => ri.RecipeId == request.RecipeId) // FILTER BY RECIPE
            .OrderBy(ri => ri.SerialNumber) // Sort by Step Number
            .Select(x => new RecipeItemDto
            {
                Id = x.Id,
                RecipeId = x.RecipeId,
                SerialNumber = x.SerialNumber,
                MaterialId = x.MaterialId,
                MaterialCode = x.Material.MaterialCode, // Map for UI
                Weight = x.Weight,
                TolerancePositive = x.TolerancePositive,
                ToleranceNegative = x.ToleranceNegative,
                Description = x.Description
            })
            .ToListAsync(token);
    }

    public async Task<int> Handle(SaveCommand<RecipeItemDto> request, CancellationToken token)
    {
        var dto = request.Data;
        RecipeItem entity;

        if (dto.Id == 0)
        {
            entity = new RecipeItem();
            _context.RecipeItems.Add(entity);
        }
        else
        {
            entity = await _context.RecipeItems.FindAsync(new object[] { dto.Id }, token);
        }

        entity.RecipeId = dto.RecipeId;
        entity.SerialNumber = dto.SerialNumber;
        entity.MaterialId = dto.MaterialId;
        entity.Weight = dto.Weight;
        entity.TolerancePositive = dto.TolerancePositive;
        entity.ToleranceNegative = dto.ToleranceNegative;
        entity.Description = dto.Description;

        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task<int> Handle(DeleteCommand<RecipeItemDto> request, CancellationToken token)
    {
        var entity = await _context.RecipeItems.FindAsync(new object[] { request.Id }, token);
        if (entity != null)
        {
            _context.RecipeItems.Remove(entity);
            await _context.SaveChangesAsync(token);
        }
        return request.Id;
    }
}