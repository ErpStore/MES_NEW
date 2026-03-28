using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MES.Domain.Entities.Recipes
{
    [Table("Recipes")]
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string RecipeName { get; set; }

        [Required]
        [MaxLength(50)]
        public string RecipeCode { get; set; }

        public int Version { get; set; }

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } // "Master" or "Final"

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public bool IsBlocked { get; set; }

        public double BatchWeight { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Navigation Property: A Recipe has many Items (Ingredients)
        public virtual ICollection<RecipeItem> RecipeItems { get; set; } = new List<RecipeItem>();
    }
}
