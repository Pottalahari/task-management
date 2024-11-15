using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace TaskMgtWebAPI.Models
{//gg
    public class UserTb
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public virtual RoleTb? Role_Obj { get; set; }

        public virtual ICollection<TaskTb> Tasks { get; set; } = new List<TaskTb>();
    }
}
