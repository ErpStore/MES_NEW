using MediatR;
using MES.ApplicationLayer.User.Commands;
using MES.ApplicationLayer.User.Dtos;
using MES.ApplicationLayer.User.Quires;
using MES.Domain.Entities;
using MES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MES.ApplicationLayer.User.Handlers
{
    // 1. GET RIGHTS FOR A USER GROUP
    public class GetUserGroupRightsHandler : IRequestHandler<GetUserGroupRightsQuery, List<UserGroupRightDto>>
    {
        private readonly MesDbContext _context;

        public GetUserGroupRightsHandler(MesDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserGroupRightDto>> Handle(GetUserGroupRightsQuery request, CancellationToken cancellationToken)
        {
            return await _context.UserGroupPermissions
                .AsNoTracking()
                .Where(r => r.UserGroupId == request.UserGroupId)
                .Select(r => new UserGroupRightDto
                {
                    Id = r.Id,
                    UserGroupId = r.UserGroupId,
                    ScreenKey = r.ScreenKey,
                    CanAdd = r.CanAdd,
                    CanEdit = r.CanEdit,
                    CanDelete = r.CanDelete
                })
                .ToListAsync(cancellationToken);
        }
    }

    // 2. SAVE RIGHTS FOR A USER GROUP (upsert: replace all rights for the group)
    public class SaveUserGroupRightsHandler : IRequestHandler<SaveUserGroupRightsCommand, bool>
    {
        private readonly MesDbContext _context;
        private readonly ILogger<SaveUserGroupRightsHandler>? _logger;

        public SaveUserGroupRightsHandler(MesDbContext context, ILogger<SaveUserGroupRightsHandler>? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(SaveUserGroupRightsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existing = await _context.UserGroupPermissions
                    .Where(r => r.UserGroupId == request.UserGroupId)
                    .ToListAsync(cancellationToken);

                _context.UserGroupPermissions.RemoveRange(existing);

                var newRights = request.Rights.Select(dto => new UserGroupPermission
                {
                    UserGroupId = request.UserGroupId,
                    ScreenKey = dto.ScreenKey,
                    CanAdd = dto.CanAdd,
                    CanEdit = dto.CanEdit,
                    CanDelete = dto.CanDelete
                }).ToList();

                await _context.UserGroupPermissions.AddRangeAsync(newRights, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

            
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to save rights for UserGroupId {Id}", request.UserGroupId);
                throw;
            }
        }
    }
}
