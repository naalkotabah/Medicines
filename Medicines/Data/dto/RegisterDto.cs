namespace Medicines.Data.dto
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; } // يجب على المستخدم اختيار الدور
    }
}
