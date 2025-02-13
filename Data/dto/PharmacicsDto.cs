namespace Medicines.Data.dto
{
    public class PharmacicsDto
    {
        public string Name { get; set; } = string.Empty; // اسم الصيدلية

        public string Address { get; set; } = string.Empty; // عنوان الصيدلية

        public decimal Latitude { get; set; } // الإحداثيات الجغرافية

        public decimal Longitude { get; set; }

        public string City { get; set; } = string.Empty; // المدينة
        public IFormFile? ImagePharmacics { get; set; }
        public string LicenseNumber { get; set; } = string.Empty; // رقم الترخيص

      
        public int PractitionerId { get; set; }

    }
}
