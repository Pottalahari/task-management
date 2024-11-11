using System.ComponentModel.DataAnnotations;

namespace TaskMgtWebAPI.DTOS
{
    public class RoleDTO
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
