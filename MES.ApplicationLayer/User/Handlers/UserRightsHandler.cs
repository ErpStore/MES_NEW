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
    // GET RIGHTS FOR A SPECIFIC USER
    public class GetUserRightsHandler : IRequestHandler<GetUserRightsQuery, List<UserRightDto>>
    {
        private readonly MesDbContext _context;

        public GetUserRightsHandler(MesDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserRightDto>> Handle(GetUserRightsQuery request, CancellationToken cancellationToken)
        {
            return await _context.UserGroupPermissions
                .AsNoTracking()
                .Where(r => r.UserGroupId == request.UserId)
                .Select(r => new UserRightDto
                {
                    Id = r.Id,
                    UserId = r.UserGroupId,
                    ScreenKey = r.ScreenKey,
                    CanAdd = r.CanAdd,
                    CanEdit = r.CanEdit,
                    CanDelete = r.CanDelete
                })
                .ToListAsync(cancellationToken);
        }
    }

    // SAVE RIGHTS FOR A SPECIFIC USER (upsert: replace all rights for the user)
    public class SaveUserRightsHandler : IRequestHandler<SaveUserRightsCommand, bool>
    {
        private readonly MesDbContext _context;
        private readonly ILogger<SaveUserRightsHandler>? _logger;

        public SaveUserRightsHandler(MesDbContext context, ILogger<SaveUserRightsHandler>? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(SaveUserRightsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existing = await _context.UserGroupPermissions
                    .Where(r => r.UserGroupId == request.UserId)
                    .ToListAsync(cancellationToken);

                _context.UserGroupPermissions.RemoveRange(existing);

                var newRights = request.Rights.Select(dto => new UserGroupPermission
                {
                    UserGroupId = request.UserId,
                    ScreenKey = dto.ScreenKey,
                    CanAdd = dto.CanAdd,
                    CanEdit = dto.CanEdit,
                    CanDelete = dto.CanDelete
                }).ToList();

                await _context.UserGroupPermissions.AddRangeAsync(newRights, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger?.LogInformation("Saved {Count} rights for UserId {Id}", newRights.Count, request.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to save rights for UserId {Id}", request.UserId);
                throw;
            }
        }
    }
}
