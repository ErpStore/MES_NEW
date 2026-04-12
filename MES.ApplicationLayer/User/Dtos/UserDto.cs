namespace MES.ApplicationLayer.User.Dtos;

// MES.Application/Users/DTOs/UserDto.cs
public class UserDto
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public int? UserGroupId { get; set; }
    public bool IsActive { get; set; }

    // Security Settings (For UI display/editing)
    public bool EnablePasswordExpiry { get; set; }
    public DateTime? PasswordExpiryDate { get; set; }

    // Computed Property: Useful for showing "John Doe" in the Grid 
    public string FullName => $"{FirstName} {LastName}".Trim();

    // NOTE: We NEVER include 'PasswordHash' in a DTO. 
    // Passwords stay in the database or are handled by specific commands.
}