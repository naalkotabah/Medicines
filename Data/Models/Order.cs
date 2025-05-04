using System.Text.Json.Serialization;

namespace Medicines.Data.Models
{
    public class Order
    {
        public int Id { get; set; } // مفتاح أساسي

        public byte Age { get; set; } //العمر 

        public float Weight { get; set; } //الوزن

        public string? ChronicDiseases { get; set; } //الأمراض المزمنة


        public DateTime OrderDate { get; set; }


        public string? DrugAllergy { get; set; } // حساسية  الادوية 

        public string? Address { get; set; }


        public int UserId { get; set; }
        public Users? User { get; set; }


        public int PharmacyId { get; set; }
        public Pharmacics? Pharmacy { get; set; }

        [JsonIgnore]
        public List<OrderMedicine> OrderMedicines { get; set; } = new();



        public OrderStatus Status { get; set; } = OrderStatus.Pending; 
        public decimal FinalPrice { get; set; }
    }



    public enum OrderStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        Canceled = 3,
        Done = 4
    }   
}
