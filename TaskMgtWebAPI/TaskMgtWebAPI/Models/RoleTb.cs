using System.ComponentModel.DataAnnotations;

namespace TaskMgtWebAPI.Models
{
    public class RoleTb
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<UserTb> Users { get; set; }
    }
}
