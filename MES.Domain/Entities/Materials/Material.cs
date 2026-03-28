using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Domain.Entities.Materials
{
    [Table("Materials")]
    public class Material
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? MaterialCode { get; set; } // Column 1

        [Required]
        [MaxLength(200)]
        public string? Name { get; set; }         // Column 2

        // Column 3: Material Group (Foreign Key)
        public int MaterialGroupId { get; set; }
        [ForeignKey(nameof(MaterialGroupId))]
        public virtual MaterialGroup? MaterialGroup { get; set; }

        [MaxLength(100)]
        public string? HandlingInfo { get; set; } // Column 4 (e.g., "Hygroscopic")

        public double Density { get; set; }       // Column 5

        [MaxLength(200)]
        public string? Manufacturer { get; set; } // Column 6

        [Required]
        [MaxLength(20)]
        public string? Unit { get; set; }          // Column 7 (e.g. "kg")

        public double MinLevel { get; set; }      // Column 8

        public int ShelfLifeDays { get; set; }    // Column 9

        public bool IsBlocked { get; set; }       // Column 10 (Checkbox)

        [MaxLength(500)]
        public string? Description { get; set; }  // Column 11
    }
}
