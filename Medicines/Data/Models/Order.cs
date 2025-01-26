namespace Medicines.Data.Models
{
    public class Order
    {
        public int Id { get; set; } // مفتاح أساسي

        public byte Age { get; set; } //العمر 

        public float Weight { get; set; } //الوزن

        public string ChronicDiseases { get; set; } //الأمراض المزمنة


        public DateTime dateTime { get; set; }
        public string DrugAllergy { get; set; } // حساسية  الادوية 

        public string Address { get; set; }

        // 🔹 العلاقة مع المستخدم (كل طلب يرتبط بمستخدم واحد)
        public int UserId { get; set; }
        public Users User { get; set; }

        // 🔹 العلاقة مع الصيدلية (كل طلب يرتبط بصيدلية واحدة)
        public int PharmacyId { get; set; }
        public Pharmacics Pharmacy { get; set; }

        // 🔹 العلاقة مع الأدوية (كل طلب يحتوي على عدة أدوية)
        public List<OrderMedicine> OrderMedicines { get; set; } 



        public decimal FinalPrice { get; set; } // السعر النهائي للطلب
    }

}
