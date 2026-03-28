using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MES.Domain.Entities.Materials
{
    [Table("MaterialGroups")]
    public class MaterialGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // The "Group Name"

        [MaxLength(255)]
        public string? Description { get; set; }

        // Navigation Property (One Group has many Materials)
        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}
