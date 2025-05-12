using System.Data;

namespace Medicines.Data.Models
{
    public class Users
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? LastName { get; set; }
        public string? UserName { get; set; }

        public string? Password { get; set; }

         public bool IsDleted { get; set; }



        public int RoleId { get; set; }
        public Roles? Role { get; set; }  // fuuuuuussssfussss

        public ICollection<Order>? Orders { get; set; } = new List<Order>();

        public Practitioner? Practitioner { get; set; }

    }

}
