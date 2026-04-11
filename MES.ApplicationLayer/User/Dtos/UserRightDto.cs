namespace MES.ApplicationLayer.User.Dtos
{
    public class UserRightDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string ScreenKey { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
