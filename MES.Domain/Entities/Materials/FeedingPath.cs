using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MES.Domain.Entities.Materials;

[Table("FeedingPaths")]
public class FeedingPath
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string BinNumber { get; set; } // e.g., "01", "02"

    [Required]
    [MaxLength(50)]
    public string BinCode { get; set; } // e.g., "#CH-BIN-01"

    // FOREIGN KEY TO MATERIAL
    public int MaterialId { get; set; }
    [ForeignKey(nameof(MaterialId))]
    public virtual Material Material { get; set; }

    public double MaxCapacity { get; set; }
    public double CurrentStock { get; set; }

    public DateTime? FilledDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}