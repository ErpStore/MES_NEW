namespace MES.ApplicationLayer.Orders.DTO
{
    public class ProductionOrderDto
    {
        public int Id { get; set; }
        public int SerialNumber { get; set; }

        public int RecipeId { get; set; }
        public string RecipeName { get; set; } // Mapped for UI Grid
        public string RecipeCode { get; set; } // Mapped for UI Grid

        public int SetBatch { get; set; }
        public int ActualBatch { get; set; }
        public string Status { get; set; }
        public bool IsReleased { get; set; }
        public DateTime? BatchStart { get; set; }
        public DateTime? BatchEnd { get; set; }
        public string? Description { get; set; }
    }
}
