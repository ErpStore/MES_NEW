using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.Materials.Dtos;
using MES.Domain.Entities.Materials;
using MES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.ApplicationLayer.Materials.Handlers
{
    public class MaterialGroupHandler :
        IRequestHandler<GetAllQuery<MaterialGroupDto>, List<MaterialGroupDto>>,
        IRequestHandler<SaveCommand<MaterialGroupDto>, int>,
        IRequestHandler<DeleteCommand<MaterialGroupDto>, int>
    {
        private readonly MesDbContext _context;

        public MaterialGroupHandler(MesDbContext context) { _context = context; }

        // GET
        public async Task<List<MaterialGroupDto>> Handle(GetAllQuery<MaterialGroupDto> request, CancellationToken token)
        {
        
            return await _context.MaterialGroups.AsNoTracking()
                .Select(x => new MaterialGroupDto { Id = x.Id, MaterialName = x.Name, MaterialDescription = x.Description })
                .ToListAsync(token);
        }

        // SAVE
        public async Task<int> Handle(SaveCommand<MaterialGroupDto> request, CancellationToken token)
        {
            var dto = request.Data;
            MaterialGroup entity;

            if (dto.Id == 0)
            {
                entity = new MaterialGroup { Name = dto.MaterialName, Description = dto.MaterialDescription };
                _context.MaterialGroups.Add(entity);
            }
            else
            {
                entity = await _context.MaterialGroups.FindAsync(new object[] { dto.Id }, token);
                if (entity == null) throw new KeyNotFoundException("Group not found");
                entity.Name = dto.MaterialName;
                entity.Description = dto.MaterialDescription;
            }
            await _context.SaveChangesAsync(token);
            return entity.Id;
        }

        // DELETE
        public async Task<int> Handle(DeleteCommand<MaterialGroupDto> request, CancellationToken token)
        {
            var entity = await _context.MaterialGroups.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                // Note: This will throw an error if Materials are linked (due to DeleteBehavior.Restrict)
                // This is GOOD. It prevents accidental data loss.
                _context.MaterialGroups.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
            return request.Id;
        }


        public async Task<List<MaterialDto>> Handle(GetAllQuery<MaterialDto> request, CancellationToken token)
        {
            return await _context.Materials.AsNoTracking()
                .Include(m => m.MaterialGroup)
                .Select(x => new MaterialDto
                {
                    Id = x.Id,
                    MaterialCode = x.MaterialCode,
                    Name = x.Name,
                    MaterialGroupId = x.MaterialGroupId,
                    MaterialGroupName = x.MaterialGroup.Name,

                    // NEW FIELDS MAPPING
                    HandlingInfo = x.HandlingInfo,
                    Density = x.Density,
                    Manufacturer = x.Manufacturer,
                    Unit = x.Unit,
                    MinLevel = x.MinLevel,
                    ShelfLifeDays = x.ShelfLifeDays,
                    IsBlocked = x.IsBlocked,
                    Description = x.Description
                })
                .ToListAsync(token);
        }

        public async Task<int> Handle(SaveCommand<MaterialDto> request, CancellationToken token)
        {
            var dto = request.Data;
            Material entity;

            if (dto.Id == 0)
            {
                entity = new Material();
                _context.Materials.Add(entity);
            }
            else
            {
                entity = await _context.Materials.FindAsync(new object[] { dto.Id }, token);
                if (entity == null) throw new KeyNotFoundException("Material not found");
            }

            // UPDATE FIELDS
            entity.MaterialCode = dto.MaterialCode;
            entity.Name = dto.Name;
            entity.MaterialGroupId = dto.MaterialGroupId;
            entity.HandlingInfo = dto.HandlingInfo;
            entity.Density = dto.Density;
            entity.Manufacturer = dto.Manufacturer;
            entity.Unit = dto.Unit;
            entity.MinLevel = dto.MinLevel;
            entity.ShelfLifeDays = dto.ShelfLifeDays;
            entity.IsBlocked = dto.IsBlocked;
            entity.Description = dto.Description;

            await _context.SaveChangesAsync(token);
            return entity.Id;
        }
    }
}
