namespace MES.ApplicationLayer.User.Dtos
{
    public class UserGroupRightDto
    {
        public int Id { get; set; }
        public int UserGroupId { get; set; }
        public required string ScreenKey { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
