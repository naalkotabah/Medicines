using System.Text.Json.Serialization;

namespace Medicines.Data.Models
{
    public class Medicine
    {
        public int Id { get; set; } // مفتاح أساسي

        public string? TradeName { get; set; } 

        public string? ScientificName { get; set; } 

        public string? ImageMedicine { get; set; }

        public decimal Dosage { get; set; } // الجرعة

        public string? DrugTiming { get; set; } 

        public string? SideEffects { get; set; } 

        public string? ContraindicatedDrugs { get; set; } 

        public string? ManufacturerName { get; set; } 

        public string? ProducingCompany { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public decimal Price { get; set; }

        [JsonIgnore]
        public ICollection<OrderMedicine> OrderMedicines { get; set; } = new List<OrderMedicine>();

        public int PharmacyId { get; set; }
        [JsonIgnore]
        public Pharmacics Pharmacy { get; set; }
    }



}
