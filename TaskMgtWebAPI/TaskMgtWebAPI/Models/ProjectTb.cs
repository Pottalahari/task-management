using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace TaskMgtWebAPI.Models
{
    public class ProjectTb
    {
        [Key]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TaskTb> Tasks { get; set; }
    }
}
