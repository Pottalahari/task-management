namespace TaskMgtWebAPI.DTOS
{
    public class TaskStatusSummaryDto
    {
        public string Status { get; set; } // Task status (e.g., Completed, In Progress)
        public double Percentage { get; set; } // Calculated percentage
    }
}
