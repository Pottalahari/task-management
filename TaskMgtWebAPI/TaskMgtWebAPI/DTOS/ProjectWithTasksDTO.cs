namespace TaskMgtWebAPI.DTOS
{
    public class ProjectWithTasksDTO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<TaskDTO> Tasks { get; set; }
    }
}
