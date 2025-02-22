
namespace TaskMgtWebAPI.DTOS
{
    public class TaskStatusUpdateDto
    {
        public string Status { get; set; }

        //public static implicit operator string(TaskStatusUpdateDto v)
        //{
        //    throw new NotImplementedException();
        //}
        public string Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
