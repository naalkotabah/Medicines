using System.Text.Json.Serialization;

namespace Medicines.Data.Models
{
    public class OrderMedicine
    {
        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }

        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }

        public int Quantity { get; set; } // الكمية المطلوبة من الدواء
    }
}
