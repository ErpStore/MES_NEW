namespace MES.ApplicationLayer.Materials.Dtos
{
    public class MaterialDto
    {
        public int Id { get; set; }
        public string? MaterialCode { get; set; }
        public string? Name { get; set; }

        // For Dropdown & Display
        public int MaterialGroupId { get; set; }
        public string? MaterialGroupName { get; set; }

        public string? HandlingInfo { get; set; }
        public double Density { get; set; }
        public string? Manufacturer { get; set; }
        public string? Unit { get; set; }
        public double MinLevel { get; set; }
        public int ShelfLifeDays { get; set; }
        public bool IsBlocked { get; set; }
        public string? Description { get; set; }
    }
}
