using System.Data;

namespace Medicines.Data.dto
{
    public class MedicinforserchDto
    {
        public int id { get; set; }
        public string? TradeName { get; set; }
        public string? ScientificName { get; set; }
        public string? ImageMedicine { get; set; }
        public decimal Dosage { get; set; }
        public string? DrugTiming { get; set; }
        public string? SideEffects { get; set; }
        public string? ContraindicatedDrugs { get; set; }
        public string? ManufacturerName { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public string? ProducingCompany { get; set; }
        public decimal Price { get; set; }
        public int PharmacyId { get; set; }
    }
}
