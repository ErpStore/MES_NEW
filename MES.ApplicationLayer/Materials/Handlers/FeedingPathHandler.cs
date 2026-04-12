using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Materials.Dtos;
using MES.Domain.Entities.Materials;
using MES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MES.ApplicationLayer.Materials.Handlers;

public class FeedingPathHandler :
    IRequestHandler<GetAllQuery<FeedingPathDto>, List<FeedingPathDto>>,
    IRequestHandler<SaveCommand<FeedingPathDto>, int>,
    IRequestHandler<DeleteCommand<FeedingPathDto>, int>
{
    private readonly MesDbContext _context;

    public FeedingPathHandler(MesDbContext context) { _context = context; }

    public async Task<List<FeedingPathDto>> Handle(GetAllQuery<FeedingPathDto> request, CancellationToken token)
    {
        return await _context.FeedingPaths.AsNoTracking()
            .Include(f => f.Material) // JOIN to get Material Code
            .Select(x => new FeedingPathDto
            {
                Id = x.Id,
                BinNumber = x.BinNumber,
                BinCode = x.BinCode,
                MaterialId = x.MaterialId,
                MaterialCode = x.Material.MaterialCode, // Map for UI
                MaxCapacity = x.MaxCapacity,
                CurrentStock = x.CurrentStock,
                FilledDate = x.FilledDate,
                ExpiryDate = x.ExpiryDate,
                Description = x.Description
            })
            .ToListAsync(token);
    }

    public async Task<int> Handle(SaveCommand<FeedingPathDto> request, CancellationToken token)
    {
        var dto = request.Data;
        FeedingPath entity;

        if (dto.Id == 0)
        {
            entity = new FeedingPath();
            _context.FeedingPaths.Add(entity);
        }
        else
        {
            entity = await _context.FeedingPaths.FindAsync(new object[] { dto.Id }, token);
        }

        entity.BinNumber = dto.BinNumber;
        entity.BinCode = dto.BinCode;
        entity.MaterialId = dto.MaterialId;
        entity.MaxCapacity = dto.MaxCapacity;
        entity.CurrentStock = dto.CurrentStock;
        entity.FilledDate = dto.FilledDate;
        entity.ExpiryDate = dto.ExpiryDate;
        entity.Description = dto.Description;

        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task<int> Handle(DeleteCommand<FeedingPathDto> request, CancellationToken token)
    {
        var entity = await _context.FeedingPaths.FindAsync(new object[] { request.Id }, token);
        if (entity != null)
        {
            _context.FeedingPaths.Remove(entity);
            await _context.SaveChangesAsync(token);
        }
        return request.Id;
    }
}