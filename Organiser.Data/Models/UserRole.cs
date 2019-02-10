using System.ComponentModel.DataAnnotations.Schema;

namespace Organiser.Data.Models
{
    public class UserRole
    {
        public int UserRoleId { get; set; }
        public int Role { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
