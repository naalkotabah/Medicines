using System.Data;

namespace Medicines.Data.Models
{
    public class Users
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

         public bool IsDleted { get; set; }



        public int RoleId { get; set; }
        public Roles Role { get; set; } 

        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }

}
