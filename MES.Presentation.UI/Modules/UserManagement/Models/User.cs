namespace MES.Presentation.UI.Modules.UserManagement.Models;

public class User
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Department { get; set; }
    public bool IsActive { get; set; }
}