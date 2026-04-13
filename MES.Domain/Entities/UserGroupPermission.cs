using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES.Domain.Entities
{
    [Table("UserGroupPermissions")] // BRAND NEW TABLE NAME
    public class UserGroupPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserGroupId { get; set; }

        [ForeignKey(nameof(UserGroupId))]
        public virtual UserGroup? UserGroup { get; set; }

        [Required]
        [MaxLength(100)]
        public required string ScreenKey { get; set; }

        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool IsVisibleForUser { get; set; } // Included the column that caused the crash earlier
    }
}
