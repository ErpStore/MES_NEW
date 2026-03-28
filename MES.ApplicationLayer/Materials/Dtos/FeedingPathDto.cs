using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.ApplicationLayer.Materials.Dtos
{
    public class FeedingPathDto
    {
        public int Id { get; set; }
        public string BinNumber { get; set; }
        public string BinCode { get; set; }

        public int MaterialId { get; set; }
        public string MaterialCode { get; set; } // For the DataGrid display

        public double MaxCapacity { get; set; }
        public double CurrentStock { get; set; }
        public DateTime? FilledDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Description { get; set; }
    }
}
