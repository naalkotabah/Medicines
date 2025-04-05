using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Medicines.Data.Models
{
    public class Pharmacics
    {
        public int Id { get; set; } // مفتاح أساسي

        public string? Name { get; set; } = string.Empty; // اسم الصيدلية

        public string? Address { get; set; } = string.Empty; // عنوان الصيدلية

        public decimal Latitude { get; set; } // الإحداثيات الجغرافية

        public decimal Longitude { get; set; }
        public string? ImagePharmacics { get; set; } 
        public string? City { get; set; } = string.Empty; // المدينة
        public bool IsActive { get; set; }
        public string? LicenseNumber { get; set; } = string.Empty; // رقم الترخيص

       

        // 🔹 علاقة الصيدلية مع الأدوية (صيدلية تحتوي على عدة أدوية)
        public List<Medicine>? Medicines { get; set; }

        public List<Order>? Orders { get; set; } = new List<Order>();

        public int PractitionerId { get; set; }

 
        public Practitioner? Practitioner { get; set; }

    }



}
