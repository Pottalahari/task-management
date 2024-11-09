using System.ComponentModel.DataAnnotations;

namespace TaskMgtWebAPI.DTOS
{
    public class ProjectDTO
    {
        [Key]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
    }
}
