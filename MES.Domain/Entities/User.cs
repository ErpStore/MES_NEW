using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MES.Domain.Entities;

[Table("Users")] // Defines the SQL Table Name
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string UserName { get; set; }

    [MaxLength(50)]
    public required string FirstName { get; set; }

    [MaxLength(50)]
    public string? LastName { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Mobile { get; set; }

    // 1. Foreign Key (The actual value stored in DB, e.g., 1, 2, 5)
    public int? UserGroupId { get; set; }

    // 2. Navigation Property (Allows accessing the Group object directly)
    [ForeignKey(nameof(UserGroupId))]
    public virtual UserGroup? UserGroup { get; set; }

    // Note: You can keep the old 'Department' string if you want to support 
    // legacy data temporarily, but ideally, you should remove it 
    // and only use UserGroup.Name.
    [NotMapped] // Tells EF Core NOT to create a column for this anymore
    public string DepartmentName => UserGroup?.Name ?? "Unknown";

    public bool IsActive { get; set; }

    // Security Fields
    public bool EnablePasswordExpiry { get; set; }
    public DateTime? PasswordExpiryDate { get; set; }

    // In a real app, store Hashed passwords, never plain text!
    [MaxLength(200)]
    public required string PasswordHash { get; set; }
}