using MediatR;
using MES.ApplicationLayer.Common;
using MES.ApplicationLayer.User.Dtos;
using MES.Domain.Entities;
using MES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MES.ApplicationLayer.User.Handlers
{
    public class UserGroupHandler :
        IRequestHandler<GetAllQuery<UserGroupDto>, List<UserGroupDto>>,
        IRequestHandler<SaveCommand<UserGroupDto>, int>,
        IRequestHandler<DeleteCommand<UserGroupDto>, int>
    {
        private readonly MesDbContext _context;
        private ILogger<UserGroupHandler>? _logger;

        public UserGroupHandler(MesDbContext context, ILogger<UserGroupHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<UserGroupDto>> Handle(GetAllQuery<UserGroupDto> request, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Handling GetAllQuery for UserGroupDto");

            try
            {
                var userGroupDto = await _context.UserGroups
                    .AsNoTracking()
                    .Select(ug => new UserGroupDto
                    {
                        Id = ug.Id,
                        Name = ug.Name,
                        Description = ug.Description
                    })
                    .ToListAsync(cancellationToken);

                _logger?.LogInformation("Successfully handled GetAllQuery for UserGroupDto");
                return userGroupDto;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while handling GetAllQuery for UserGroupDto");
                throw;
            }
        }

        public async Task<int> Handle(SaveCommand<UserGroupDto> request, CancellationToken cancellationToken)
        {
            var dto = request.Data;

            UserGroup? userGroup = null;

            _logger?.LogInformation("Handling Save UserGroup");

            if (dto.Id == 0)
            {
                userGroup = new UserGroup
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    CreatedOn = DateTime.Now,
                    CreatedBy = dto.UserName
                };

                _context.UserGroups.Add(userGroup);
                _logger?.LogInformation("Saving the new record");
            }
            else
            {
                userGroup = await _context.UserGroups.FindAsync([dto.Id], cancellationToken);

                if (userGroup != null)
                {
                    userGroup.Name = dto.Name;
                    userGroup.Description = dto.Description;
                    userGroup.ModifiedOn = DateTime.Now;
                    userGroup.ModifiedBy = dto.UserName;

                    _logger?.LogInformation("Updating the existing record");
                }
                else
                {
                    _logger?.LogWarning($"UserGroup with Id {dto.Id} not found for update.");
                    throw new InvalidOperationException($"UserGroup with Id {dto.Id} not found.");
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return userGroup.Id;
        }

        public async Task<int> Handle(DeleteCommand<UserGroupDto> request, CancellationToken cancellationToken)
        {
            var userGroupId = request.Id;
            _logger?.LogInformation($"Handling Delete UserGroup with Id {userGroupId}");

            var userGroup = await _context.UserGroups.FindAsync([userGroupId], cancellationToken);

            if (userGroup != null)
            {
                _logger?.LogInformation("Deleting the record");
                _context.UserGroups.Remove(userGroup);

                await _context.SaveChangesAsync(cancellationToken);
            }

            return userGroupId;
        }
    }
}
