using MES.Domain.Entities.Materials;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MES.Domain.Entities.Recipes
{
    [Table("RecipeItems")]
    public class RecipeItem
    {
        [Key]
        public int Id { get; set; }

        // FOREIGN KEY: Links back to the Recipe
        public int RecipeId { get; set; }
        [ForeignKey(nameof(RecipeId))]
        public virtual Recipe Recipe { get; set; }

        public int SerialNumber { get; set; } // Matches "S.No." column

        // FOREIGN KEY: Links to the Material Management table
        public int MaterialId { get; set; }
        [ForeignKey(nameof(MaterialId))]
        public virtual Material Material { get; set; }

        public double Weight { get; set; }

        public double TolerancePositive { get; set; } // Tol +

        public double ToleranceNegative { get; set; } // Tol -

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
