namespace MES.ApplicationLayer.User.Dtos
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public UserDto? User { get; set; }
        public List<UserGroupRightDto> Rights { get; set; } = new();
        public List<UserRightDto> UserRights { get; set; } = new();
    }
}
