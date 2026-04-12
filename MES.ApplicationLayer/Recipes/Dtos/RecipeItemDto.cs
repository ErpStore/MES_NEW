namespace MES.ApplicationLayer.Recipes.Dtos;

public class RecipeItemDto
{
    public int Id { get; set; }
    public int RecipeId { get; set; }

    public int SerialNumber { get; set; }

    // Material Dropdown Link
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } // Displayed in Grid

    public double Weight { get; set; }
    public double TolerancePositive { get; set; }
    public double ToleranceNegative { get; set; }
    public string? Description { get; set; }
}