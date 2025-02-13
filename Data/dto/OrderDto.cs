using Medicines.Data.Models;

namespace Medicines.Data.dto
{
    public class OrderDto
    {
        public byte Age { get; set; }
        public float Weight { get; set; }
        public string ChronicDiseases { get; set; }
        public string DrugAllergy { get; set; }
        public string Address { get; set; }
        public int UserId { get; set; }
        public int PharmacyId { get; set; }
        public List<int> MedicineIds { get; set; } // قائمة معرفات الأدوية

    }


}
