namespace MES.ApplicationLayer.Materials.Dtos;

public class MaterialGroupDto
{
    public int Id { get; set; }
    public string? MaterialName { get; set; }        // Matches Col 1: "Material Group"
    public string? MaterialDescription { get; set; } // Matches Col 2: "Description"
}