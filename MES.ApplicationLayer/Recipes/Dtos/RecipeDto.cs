namespace MES.ApplicationLayer.Recipes.Dtos;

public class RecipeDto
{
    public int Id { get; set; }
    public string RecipeName { get; set; }
    public string RecipeCode { get; set; }
    public int Version { get; set; }
    public string Type { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public bool IsBlocked { get; set; }
    public double BatchWeight { get; set; }
    public string? Description { get; set; }
}