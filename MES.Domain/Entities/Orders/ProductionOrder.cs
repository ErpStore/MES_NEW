using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MES.Domain.Entities.Recipes;

namespace MES.Domain.Entities.Orders;

[Table("ProductionOrders")]
public class ProductionOrder
{
    [Key]
    public int Id { get; set; }

    public int SerialNumber { get; set; } // Matches "S.No."

    // FOREIGN KEY: Links to the Recipe
    public int RecipeId { get; set; }
    [ForeignKey(nameof(RecipeId))]
    public virtual Recipe Recipe { get; set; }

    public int SetBatch { get; set; }

    public int ActualBatch { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Not Started"; // e.g., Not Started, In Progress, Completed

    public bool IsReleased { get; set; } // Matches the "Release" checkbox

    public DateTime? BatchStart { get; set; }
    public DateTime? BatchEnd { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}