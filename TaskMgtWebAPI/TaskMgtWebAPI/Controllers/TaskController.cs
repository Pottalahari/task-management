using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgtWebAPI.DTOS;
using TaskMgtWebAPI.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Entity;
using System.Net.Mail;

namespace TaskMgtWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskMgtDBContext _context;
        private readonly ILogger<TaskController> _logger;

        public TaskController(TaskMgtDBContext context, ILogger<TaskController> logger)
        {
            _context = context;
            _logger = logger;
        }
        // http get method 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskTb>>> GetTasks()
        {
            try
            {
                _logger.LogInformation("Fetching all tasks from the database.");
                var tasks = await _context.TaskTb.ToListAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching tasks.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching tasks.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskTb>> GetTaskById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching task with ID: {Id}", id);
                var task = await _context.TaskTb.FindAsync(id);

                if (task == null)
                {
                    _logger.LogWarning("Task with ID: {Id} not found.", id);
                    return NotFound($"Task with ID {id} not found.");
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the task.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the task.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskTb>> CreateTask(TaskDTO taskDTO)
        {
            try
            {
                _logger.LogInformation("Creating a new task.");

                // we are validating the user 
                var assignedUser = await _context.UserTb.FindAsync(taskDTO.UserId);
                if (assignedUser == null)
                {
                    return NotFound("Assigned user does not exist.");
                }

                var task = new TaskTb
                {
                    TaskId = taskDTO.TaskId,
                    Title = taskDTO.Title,
                    Description = taskDTO.Description,
                    UserId = taskDTO.UserId,
                    DueDate = taskDTO.DueDate ?? default,
                    Priority = taskDTO.Priority,
                    ProjectId = taskDTO.ProjectId,
                    Status = taskDTO.Status
                };

                _context.TaskTb.Add(task);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTaskById), new { id = task.TaskId }, task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the task.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the task.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskDTO taskDTO)
        {
            if (id != taskDTO.TaskId)
            {
                return BadRequest("Task ID mismatch.");
            }

            var task = await _context.TaskTb.FindAsync(id);
            if (task == null)
            {
                return NotFound($"Task with ID {id} not found.");
            }

            try
            {
                _logger.LogInformation("Updating task with ID: {Id}", id);

                task.Title = taskDTO.Title;
                task.Description = taskDTO.Description;
                task.UserId = taskDTO.UserId;
                task.DueDate = taskDTO.DueDate ?? default;
                task.Priority = taskDTO.Priority;
                task.ProjectId = taskDTO.ProjectId;
                task.Status = taskDTO.Status;

                _context.Entry(task).State =EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the task.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the task.");
            }
        }
        //[HttpPatch("{id}")]
        //public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] TaskStatusUpdateDto request)
        //{
        //    if (string.IsNullOrEmpty(request?.Status))
        //    {
        //        return BadRequest("Status is required.");
        //    }

        //    var task = await _context.TaskTb.FindAsync(id);
        //    if (task == null)
        //    {
        //        return NotFound("Task not found.");
        //    }

        //    task.Status = request.Status;
        //    task.Priority = request.Priority; // Update priority
        //    task.DueDate = (DateTime)request.DueDate; // Update due date

        //    await _context.SaveChangesAsync();

        //    return NoContent(); // 204 No Content response if successful
        //}


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                _logger.LogInformation("Deleting task with ID: {Id}", id);

                var task = await _context.TaskTb.FindAsync(id);
                if (task == null)
                {
                    return NotFound("Task not found.");
                }

                _context.TaskTb.Remove(task);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the task.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the task.");
            }
        }
        [HttpGet("getProjectTasks/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            // Fetch tasks for the specified project ID
            var projectTasks = await _context.TaskTb
                .Where(t => t.ProjectId == projectId) // Filter by ProjectId
                .Select(task => new
                {
                    task.TaskId,
                    task.Title,
                    task.Description,
                    task.DueDate,
                    task.Priority,
                    task.Status,
                    task.UserId
                })
                .ToListAsync();

            // Check if tasks exist for the given project ID
            if (projectTasks == null || projectTasks.Count == 0)
            {
                return NotFound(new { Message = $"No tasks found for Project ID: {projectId}" });
            }

            // Return the tasks as a response
            return Ok(projectTasks);
        }
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] TaskStatusUpdateDto request)
    {
        if (string.IsNullOrEmpty(request?.Status))
        {
            return BadRequest("Status is required.");
        }

        var task = await _context.TaskTb.FindAsync(id);
        if (task == null)
        {
            return NotFound("Task not found.");
        }

        try
        {
            // Update the task status and other properties
            task.Status = request.Status;
            task.Priority = request.Priority; // Update priority
            task.DueDate = (DateTime)request.DueDate; // Update due date

            await _context.SaveChangesAsync();

            // Fetch the user to whom the task is assigned
            //var user = await _context.UserTb.FindAsync(task.UserId);
            //if (user != null && !string.IsNullOrEmpty(user.Email))
            //{
            //    // Send an email notification
            //    SendEmailNotification(user.Email, task.TaskId, task.Status, task.Priority, (DateTime)task.DueDate);
            //}

            return NoContent(); // 204 No Content response if successful
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the task.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the task.");
        }
    }

        //private void SendEmailNotification(string toEmail, int taskId, string status, string priority, DateTime dueDate)
        //{
        //    string fromEmail = "laharipotta7@gmail.com"; // Replace with your email
        //    string subject = "Task Update Notification";
        //    string body = $@"
        //    <p>Hello,</p>
        //    <p>The following task has been updated:</p>
        //    <ul>
        //        <li><strong>Task ID:</strong> {taskId}</li>
        //        <li><strong>Status:</strong> {status}</li>
        //        <li><strong>Priority:</strong> {priority}</li>
        //        <li><strong>Due Date:</strong> {dueDate.ToShortDateString()}</li>
        //    </ul>
        //    <p>Thank you!</p>";

        //    using (MailMessage mail = new MailMessage())
        //    {
        //        mail.From = new MailAddress(fromEmail);
        //        mail.To.Add(toEmail);
        //        mail.Subject = subject;
        //        mail.Body = body;
        //        mail.IsBodyHtml = true; // Enable HTML formatting

        //        using (SmtpClient smtp = new SmtpClient("smtp.example.com", 587)) // Replace with your SMTP server and port
        //        {
        //            smtp.Credentials = new System.Net.NetworkCredential("laharipotta7@gmail.com", ""); // Replace with your credentials
        //            smtp.EnableSsl = true;
        //            smtp.Send(mail);
        //        }
        //    }
        //}

        [HttpGet("downloadProjectReport/{projectId}")]
        public async Task<IActionResult> DownloadProjectReport(int projectId)
        {
            // Fetch the project details
            var project = await _context.ProjectTb
                .Where(p => p.ProjectId == projectId)
                .Select(p => new
                {
                    p.ProjectName,
                    p.Description,
                    p.StartDate,
                    p.EndDate,
                    Tasks = _context.TaskTb
                        .Where(t => t.ProjectId == projectId)
                        .Select(t => new
                        {
                            t.TaskId,
                            t.Description,
                            t.DueDate,
                            t.Priority,
                            t.Status
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound($"No project found with ID: {projectId}");
            }

            // Generate CSV content
            var reportContent = new StringBuilder();
            reportContent.AppendLine($"Project Name: {project.ProjectName}");
            reportContent.AppendLine($"Description: {project.Description}");
            reportContent.AppendLine($"Start Date: {project.StartDate:yyyy-MM-dd}");
            reportContent.AppendLine($"End Date: {project.EndDate:yyyy-MM-dd}");
            reportContent.AppendLine("Tasks:");
            reportContent.AppendLine("Task Name,Description,Due Date,Priority,Status");

            foreach (var task in project.Tasks)
            {
                reportContent.AppendLine($"{task.TaskId},{task.Description},{task.DueDate:yyyy-MM-dd},{task.Priority},{task.Status}");
            }

            var reportBytes = Encoding.UTF8.GetBytes(reportContent.ToString());
            var fileName = $"Project_{projectId}_Report.csv";

            return File(reportBytes, "text/csv", fileName);
        }
        [HttpGet("UserProductivityReport/{userId}")]
        public async Task<IActionResult> DownloadUserProductivityReport(int userId)
        {
            // Fetch user details
            var user = await _context.UserTb
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    UserId = u.UserId,
                    UserName = u.FirstName,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {userId} not found." });
            }

            // Fetch tasks for the user
            var tasks = await _context.TaskTb
                .Where(t => t.UserId == userId)
                .Select(t => new
                {
                    TaskName = t.Title,
                    Status = t.Status
                })
                .ToListAsync();

            if (tasks == null || tasks.Count == 0)
            {
                return NotFound(new { Message = "No tasks found for this user." });
            }

            // Calculate productivity
            int totalTasks = tasks.Count;
            int completedTasks = tasks.Count(t => t.Status == "Completed");
            int inProgressTasks = tasks.Count(t => t.Status == "In Progress");
            int notstarted = tasks.Count(t => t.Status == "Not Started");

            double productivity = (completedTasks * 100) + (inProgressTasks * 50);
            double productivityPercentage = totalTasks > 0 ? productivity / totalTasks : 0;

            // Generate txt content
            var tContent = new StringBuilder();
            tContent.AppendLine("User Productivity Report");
            tContent.AppendLine($"User Name: {user.UserName}");
            tContent.AppendLine($"Email: {user.Email}");
            tContent.AppendLine($"Total Tasks: {totalTasks}");
            tContent.AppendLine($"Completed Tasks: {completedTasks}");
            tContent.AppendLine($"In Progress Tasks: {inProgressTasks}");
            tContent.AppendLine($"In Progress  Tasks: {notstarted}");
            tContent.AppendLine($"Productivity Percentage: {productivityPercentage:F2}%");
            tContent.AppendLine();
            tContent.AppendLine("Task Name,Status");

            foreach (var task in tasks)
            {
                tContent.AppendLine($"{task.TaskName},{task.Status}");
            }

            // Convert tcontent to bytes and return as file
            var tBytes = Encoding.UTF8.GetBytes(tContent.ToString());
            var fileName = $"User_{userId}_Productivity_Report.txt";

            return File(tBytes, "text/txt", fileName);
        }

        //[HttpGet("DownloadUserProductivityReport/{userId}")]
        //public async Task<IActionResult> DownloadUserProductivityReport(int userId)
        //{
        //    // Fetch user details
        //    var user = await _context.UserTb
        //        .Where(u => u.UserId == userId)
        //        .Select(u => new
        //        {
        //            UserId = u.UserId,
        //            UserName = u.FirstName,
        //            Email = u.Email
        //        })
        //        .FirstOrDefaultAsync();

        //    if (user == null)
        //    {
        //        return NotFound(new { Message = $"User with ID {userId} not found." });
        //    }

        //    // Fetch tasks for the user
        //    var tasks = await _context.TaskTb
        //        .Where(t => t.UserId == userId)
        //        .Select(t => new
        //        {
        //            TaskName = t.Title,
        //            Status = t.Status
        //        })
        //        .ToListAsync();

        //    if (tasks == null || tasks.Count == 0)
        //    {
        //        return NotFound(new { Message = "No tasks found for this user." });
        //    }

        //    // Calculate productivity
        //    int totalTasks = tasks.Count;
        //    int completedTasks = tasks.Count(t => t.Status == "Completed");
        //    int inProgressTasks = tasks.Count(t => t.Status == "In Progress");
        //    int notstarted = tasks.Count(t => t.Status == "Not Started");

        //    double productivity = (completedTasks * 100) + (inProgressTasks * 50);
        //    double productivityPercentage = totalTasks > 0 ? productivity / totalTasks : 0;

        //    // Generate CSV content
        //    var csvContent = new StringBuilder();
        //    csvContent.AppendLine("User Productivity Report");
        //    csvContent.AppendLine($"User Name: {user.UserName}");
        //    csvContent.AppendLine($"Email: {user.Email}");
        //    csvContent.AppendLine($"Total Tasks: {totalTasks}");
        //    csvContent.AppendLine($"Completed Tasks: {completedTasks}");
        //    csvContent.AppendLine($"In Progress Tasks: {inProgressTasks}");
        //    csvContent.AppendLine($"In Progress Tasks: {notstarted}");
        //    csvContent.AppendLine($"Productivity Percentage: {productivityPercentage:F2}%");
        //    csvContent.AppendLine();
        //    csvContent.AppendLine("Task Name,Status");

        //    foreach (var task in tasks)
        //    {
        //        csvContent.AppendLine($"{task.TaskName},{task.Status}");
        //    }

        //    // Convert CSV content to bytes and return as file
        //    var csvBytes = Encoding.UTF8.GetBytes(csvContent.ToString());
        //    var fileName = $"User_{userId}_Productivity_Report.csv";

        //    return File(csvBytes, "text/csv", fileName);
        //}



        [HttpGet("ProjectTaskStatusReport/{projectId}")]
        public async Task<IActionResult> DownloadProjectTaskStatusReport(int projectId)
        {
            // Fetch project details
            var project = await _context.ProjectTb
                .Where(p => p.ProjectId == projectId)
                .Select(p => new
                {
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                })
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound(new { Message = $"Project with ID {projectId} not found." });
            }

            // Fetch tasks for the project and categorize by status
            var tasks = await _context.TaskTb
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();

            var completedTasks = tasks.Where(t => t.Status == "Completed").ToList();
            var inProgressTasks = tasks.Where(t => t.Status == "In Progress").ToList();
            var notStartedTasks = tasks.Where(t => t.Status == "Not Started").ToList();

            // Generate CSV content
            var csvContent = new StringBuilder();
            csvContent.AppendLine("Project Task Status Report");
            csvContent.AppendLine($"Project Name: {project.ProjectName}");
            csvContent.AppendLine($"Description: {project.Description}");
            csvContent.AppendLine($"Start Date: {project.StartDate:yyyy-MM-dd}");
            csvContent.AppendLine($"End Date: {project.EndDate:yyyy-MM-dd}");
            csvContent.AppendLine();

            // Append tasks by status
            csvContent.AppendLine("Completed Tasks:");
            csvContent.AppendLine("Task ID,Task Name,Due Date,Priority");
            foreach (var task in completedTasks)
            {
                csvContent.AppendLine($"{task.TaskId},{task.Title},{task.DueDate:yyyy-MM-dd},{task.Priority}");
            }

            csvContent.AppendLine();
            csvContent.AppendLine("In Progress Tasks:");
            csvContent.AppendLine("Task ID,Task Name,Due Date,Priority");
            foreach (var task in inProgressTasks)
            {
                csvContent.AppendLine($"{task.TaskId},{task.Title},{task.DueDate:yyyy-MM-dd},{task.Priority}");
            }

            csvContent.AppendLine();
            csvContent.AppendLine("Not Started Tasks:");
            csvContent.AppendLine("Task ID,Task Name,Due Date,Priority");
            foreach (var task in notStartedTasks)
            {
                csvContent.AppendLine($"{task.TaskId},{task.Title},{task.DueDate:yyyy-MM-dd},{task.Priority}");
            }

            // Convert CSV content to bytes and return as file
            var csvBytes = Encoding.UTF8.GetBytes(csvContent.ToString());
            var fileName = $"Project_{projectId}_Task_Status_Report.csv";

            return File(csvBytes, "text/csv", fileName);
        }


    }
}
