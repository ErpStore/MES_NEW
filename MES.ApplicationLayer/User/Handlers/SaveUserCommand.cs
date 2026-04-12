using MediatR;

namespace MES.ApplicationLayer.User.Handlers;

// Request returns 'int' (The ID of the saved user)
public class SaveUserCommand : IRequest<int>
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public int? UserGroupId { get; set; }

    public bool IsActive { get; set; }
    public bool EnablePasswordExpiry { get; set; }
    public DateTime? PasswordExpiryDate { get; set; }

    // Optional: Password field for creating new users
    public string? Password { get; set; }
}