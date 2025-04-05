namespace Medicines.Data.Models
{
    public class Roles
    {
     
            public int Id { get; set; }
            public string? Name { get; set; } // مثال: "Admin", "User", "Manager"

            public ICollection<Users> Users { get; set; } = new List<Users>();
        

    }
}
