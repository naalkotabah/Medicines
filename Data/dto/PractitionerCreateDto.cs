namespace Medicines.Data.dto
{
    public class PractitionerCreateDto
    {
        
            public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
            public string? PhonNumber { get; set; }
            public string? Studies { get; set; }

        public IFormFile? ImagePractitioner { get; set; } //صورة لشهادة الصيدلية 
    }
}
