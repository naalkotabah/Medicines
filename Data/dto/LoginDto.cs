namespace Medicines.Data.dto
{
    public class LoginDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Token { get; set; }

        // اختياري للصيدلاني
        public int? PractitionerId { get; set; }
        public string? PractitionerName { get; set; }

        public object? Pharmacy { get; set; }
    }

}
