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
    public class MaterialHandler :
         IRequestHandler<GetAllQuery<MaterialDto>, List<MaterialDto>>,
         IRequestHandler<SaveCommand<MaterialDto>, int>,
         IRequestHandler<DeleteCommand<MaterialDto>, int>
    {
        private readonly MesDbContext _context;

        public MaterialHandler(MesDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. GET ALL
        // ==========================================
        public async Task<List<MaterialDto>> Handle(GetAllQuery<MaterialDto> request, CancellationToken cancellationToken)
        {
            return await _context.Materials.AsNoTracking()
                // CRITICAL: Include joins the MaterialGroup table so we can display the Group Name in the DataGrid
                .Include(m => m.MaterialGroup)
                .Select(x => new MaterialDto
                {
                    Id = x.Id,
                    MaterialCode = x.MaterialCode,
                    Name = x.Name,

                    // Dropdown / Navigation mapping
                    MaterialGroupId = x.MaterialGroupId,
                    MaterialGroupName = x.MaterialGroup.Name,

                    // Detail fields
                    HandlingInfo = x.HandlingInfo,
                    Density = x.Density,
                    Manufacturer = x.Manufacturer,
                    Unit = x.Unit,
                    MinLevel = x.MinLevel,
                    ShelfLifeDays = x.ShelfLifeDays,
                    IsBlocked = x.IsBlocked,
                    Description = x.Description
                })
                .ToListAsync(cancellationToken);
        }

        // ==========================================
        // 2. SAVE (Insert or Update)
        // ==========================================
        public async Task<int> Handle(SaveCommand<MaterialDto> request, CancellationToken cancellationToken)
        {
            var dto = request.Data;
            Material entity;

            if (dto.Id == 0)
            {
                // ADD NEW MODE
                entity = new Material(); 
                _context.Materials.Add(entity);
            }
            else
            {
                // UPDATE EXISTING MODE
                entity = await _context.Materials.FindAsync(new object[] { dto.Id }, cancellationToken);

                if (entity == null)
                    throw new KeyNotFoundException($"Material with ID {dto.Id} not found in the database.");
            }

            // Map the properties from the UI (DTO) to the Database (Entity)
            entity.MaterialCode = dto.MaterialCode;
            entity.Name = dto.Name;
            entity.MaterialGroupId = dto.MaterialGroupId; // Saves the selected Dropdown ID
            entity.HandlingInfo = dto.HandlingInfo;
            entity.Density = dto.Density;
            entity.Manufacturer = dto.Manufacturer;
            entity.Unit = dto.Unit;
            entity.MinLevel = dto.MinLevel;
            entity.ShelfLifeDays = dto.ShelfLifeDays;
            entity.IsBlocked = dto.IsBlocked;
            entity.Description = dto.Description;

            // Commit to SQL
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }

        // ==========================================
        // 3. DELETE
        // ==========================================
        public async Task<int> Handle(DeleteCommand<MaterialDto> request, CancellationToken cancellationToken)
        {
            var entity = await _context.Materials.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                _context.Materials.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return request.Id;
        }
    }
}
