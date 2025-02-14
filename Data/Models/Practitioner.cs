using Newtonsoft.Json;

namespace Medicines.Data.Models
{
    public class Practitioner
    {
        public int Id { get; set; }
        public string NamePractitioner { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string PhonNumber { get; set; }
        public string Studies { get; set; }
        public bool? IsDleted { get; set; }
      
        public string ImagePractitioner { get; set; } //صورة لشهادة الصيدلية 

        // 🔹 علاقة One-to-One إلزامية
        public Pharmacics? Pharmacy { get; set; }
    }

}
