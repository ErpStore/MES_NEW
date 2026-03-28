namespace MES.ApplicationLayer.User.Dtos
{
    public class UserGroupDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public string? UserName { get; set; } = null;
    }
}
