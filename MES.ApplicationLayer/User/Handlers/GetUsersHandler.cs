using MediatR;
using MES.ApplicationLayer.User.Dtos;
using MES.ApplicationLayer.User.Quires;
using MES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MES.ApplicationLayer.User.Commands;


namespace MES.ApplicationLayer.User.Handlers;

// 1. GET USER LIST HANDLER
public class GetUsersHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly MesDbContext _context;


    public GetUsersHandler(MesDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking() // Performance tip: Read-only query is faster
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,     // Mapping Entity -> DTO
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Mobile = u.Mobile,
                //Department = u.UserGroup.Name,
                IsActive = u.IsActive,
                EnablePasswordExpiry = u.EnablePasswordExpiry,
                PasswordExpiryDate = u.PasswordExpiryDate
            })
            .ToListAsync(cancellationToken);
    }
}

// 2. SAVE USER HANDLER (Handles Add & Update)
public class SaveUserHandler : IRequestHandler<SaveUserCommand, int>
{
    private readonly MesDbContext _context;

    public SaveUserHandler(MesDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(SaveUserCommand request, CancellationToken cancellationToken)
    {
        MES.Domain.Entities.User userEntity;

        if (request.Id > 0)
        {
            // --- UPDATE ---
            userEntity = await _context.Users.FindAsync([request.Id], cancellationToken);

            if (userEntity == null) return 0;

            // Update ALL fields
            userEntity.UserName = request.UserName;
            userEntity.Email = request.Email;
            userEntity.UserGroupId = request.UserGroupId;
            userEntity.IsActive = request.IsActive;

            // New Fields (Now Uncommented)
            userEntity.FirstName = request.FirstName;
            userEntity.LastName = request.LastName;
            userEntity.Mobile = request.Mobile;
            userEntity.EnablePasswordExpiry = request.EnablePasswordExpiry;
            userEntity.PasswordExpiryDate = request.PasswordExpiryDate;
        }
        else
        {
            // --- ADD ---
            userEntity = new MES.Domain.Entities.User
            {
                UserName = request.UserName,
                Email = request.Email,
                UserGroupId = request.UserGroupId,
                IsActive = request.IsActive,

                // New Fields
                FirstName = request.FirstName,
                LastName = request.LastName,
                Mobile = request.Mobile,
                EnablePasswordExpiry = request.EnablePasswordExpiry,
                PasswordExpiryDate = request.PasswordExpiryDate,

                // Set Password (use the one from UI or default)
                PasswordHash = string.IsNullOrEmpty(request.Password) ? "Default123!" : request.Password
            };

            await _context.Users.AddAsync(userEntity, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return userEntity.Id;
    }
}


// 3. DELETE USER HANDLER
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, int>
{
    private readonly MesDbContext _context;

    public DeleteUserHandler(MesDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Find the user
        var user = await _context.Users.FindAsync([request.Id], cancellationToken);

        if (user == null) return 0; // Or throw exception

        // 2. Delete
        _context.Users.Remove(user);

        // 3. Commit to SQL
        await _context.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}