using MediatR;
using MES.ApplicationLayer.User.Dtos;
using MES.ApplicationLayer.User.Quires;
using MES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MES.ApplicationLayer.User.Handlers
{
    public class LoginHandler : IRequestHandler<LoginQuery, LoginResultDto>
    {
        private readonly MesDbContext _context;
        private readonly ILogger<LoginHandler>? _logger;

        public LoginHandler(MesDbContext context, ILogger<LoginHandler>? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LoginResultDto> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.UserGroup)
                    .FirstOrDefaultAsync(u => u.UserName == request.UserName && u.IsActive, cancellationToken);

                if (user == null)
                {
                    return new LoginResultDto { Success = false, ErrorMessage = "User not found or inactive." };
                }

                // Simple password check (plain-text or stored as-is – same approach used in SaveUserHandler)
                if (user.PasswordHash != request.Password)
                {
                    return new LoginResultDto { Success = false, ErrorMessage = "Invalid password." };
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Mobile = user.Mobile,
                    UserGroupId = user.UserGroupId,
                    IsActive = user.IsActive,
                    EnablePasswordExpiry = user.EnablePasswordExpiry,
                    PasswordExpiryDate = user.PasswordExpiryDate
                };

                List<UserGroupRightDto> rights = new();

                if (user.UserGroupId.HasValue)
                {
                    rights = await _context.UserGroupRights
                        .AsNoTracking()
                        .Where(r => r.UserGroupId == user.UserGroupId.Value)
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

                var userRights = await _context.UserRights
                    .AsNoTracking()
                    .Where(r => r.UserId == user.Id)
                    .Select(r => new UserRightDto
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        ScreenKey = r.ScreenKey,
                        CanAdd = r.CanAdd,
                        CanEdit = r.CanEdit,
                        CanDelete = r.CanDelete
                    })
                    .ToListAsync(cancellationToken);

                _logger?.LogInformation("User {UserName} logged in successfully.", request.UserName);

                return new LoginResultDto
                {
                    Success = true,
                    User = userDto,
                    Rights = rights,
                    UserRights = userRights
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Login error for user {UserName}", request.UserName);
                return new LoginResultDto { Success = false, ErrorMessage = "An error occurred during login." };
            }
        }
    }
}
