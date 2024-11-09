using System.ComponentModel.DataAnnotations;

namespace TaskMgtWebAPI.DTOS
{
    public class ProjectDTO
    {
        [Key]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
    }
}
