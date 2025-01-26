using System.Text.Json.Serialization;

namespace Medicines.Data.Models
{
    public class Medicine
    {
        public int Id { get; set; } // مفتاح أساسي

        public string TradeName { get; set; } = string.Empty; // الاسم التجاري للدواء

        public string ScientificName { get; set; } = string.Empty; // الاسم العلمي

        public string ImageMedicine { get; set; } = string.Empty; // صورة الدواء

        public decimal Dosage { get; set; } // الجرعة

        public string DrugTiming { get; set; } = string.Empty; // توقيت الدواء

        public string SideEffects { get; set; } = string.Empty; // الآثار الجانبية

        public string ContraindicatedDrugs { get; set; } = string.Empty; // الأدوية الممنوعة مع هذا الدواء

        public string ManufacturerName { get; set; } = string.Empty; // اسم المصنع

        public string ProducingCompany { get; set; } = string.Empty; // اسم الشركة المنتجة

        public decimal Price { get; set; }


        public ICollection<OrderMedicine> OrderMedicines { get; set; } = new List<OrderMedicine>();

        public int PharmacyId { get; set; }
        [JsonIgnore]
        public Pharmacics Pharmacy { get; set; }
    }



}
