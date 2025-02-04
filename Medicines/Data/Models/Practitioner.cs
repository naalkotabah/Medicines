using Newtonsoft.Json;

namespace Medicines.Data.Models
{
    public class Practitioner
    {
        public int Id { get; set; }
        public string NamePractitioner { get; set; }
        public string Address { get; set; }
        public string PhonNumber { get; set; }
        public string Studies { get; set; }

        // 🔹 علاقة One-to-One إلزامية
        public Pharmacics? Pharmacy { get; set; }
    }

}
