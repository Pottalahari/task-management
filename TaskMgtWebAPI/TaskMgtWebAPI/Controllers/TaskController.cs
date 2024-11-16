using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgtWebAPI.DTOS;
using TaskMgtWebAPI.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace TaskMgtWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskMgtDBContext _context;

        //property for logger
        private readonly ILogger<TaskController> _logger;

        public TaskController(TaskMgtDBContext context, ILogger<TaskController> logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        /// Retrieves all tasks from the TaskTb table.
        /// </summary>
        /// <returns>A list of all tasks.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskTb>>> GetTasks()
        {
            _logger.LogInformation("Fetching all tasks from the database");

            try
            {
                _logger.LogInformation(" Recevied a get request");
                // Retrieve all projects from the ProjectTb table asynchronously.

                var task = await _context.TaskTb.ToListAsync();
                return Ok(task);
            }
            catch (Exception ex)
            {
                // Return a bad request response with an error message if an exception occurs.
                return BadRequest($"An error occured: {ex.Message}");
            }

        }

        /// <summary>
        /// Retrieves a task by its ID from the TaskTb table.
        /// </summary>
        /// <param name="id">The ID of the task to retrieve.</param>
        /// <returns>The task with the specified ID, or a NotFound status if not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskTb>> GetTaskById(int id)
        {
            // it is retrive the data from database by Id
            try
            {
                // Find the project by its ID asynchronously.
                var TaskTb = await _context.TaskTb.FindAsync(id);

                // If the project is not found, return a NotFound response.
                if (TaskTb == null)
                {
                    _logger.LogWarning("Task with ID {Id} not found.", id);
                    return NotFound($"Task with ID {id} not found");
                }
                return Ok(TaskTb);

            }
            catch (Exception ex)
            {
                return BadRequest($"an error is occured: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing task in the TaskTb table.
        /// </summary>
        /// <param name="id">The ID of the task to update.</param>
        /// <param name="TaskDTO">The task data transfer object containing updated task details.</param>
        /// <returns>Returns NoContent status if the update is successful, or an error message if the update fails.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskDTO TaskDTO)
        {
            if (id != TaskDTO.TaskId)
            {
                return BadRequest();
            }

            TaskTb taskTb = new TaskTb()
            {
                TaskId = TaskDTO.TaskId,
                Description = TaskDTO.Description,
                UserId = TaskDTO.UserId,
                DueDate = (DateTime)TaskDTO.DueDate,
                Priority = TaskDTO.Priority,
                ProjectId = TaskDTO.ProjectId,
                Status = TaskDTO.Status,
                Title = TaskDTO.Title
            };


            _context.Entry(taskTb).State = EntityState.Modified;
            try
            {
                _logger.LogInformation("Updating task with TaskId: {TaskId}", taskTb.TaskId);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskTbExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        private bool TaskTbExists(int id)
        {
            return _context.TaskTb.Any(e => e.TaskId == id);
        }

        /// <summary>
        /// Creates a new task in the TaskTb table.
        /// </summary>
        /// <param name="TaskDTO">The task data transfer object containing task details.</param>
        /// <returns>The newly created task if successful.</returns>
        [HttpPost]
        //Method to create a new ProjectTb record in the database
        public async Task<ActionResult<TaskTb>> CreateTask(TaskDTO TaskDTO)
        {
            // This object will store the properties provided in the ProjectDTO and be used to insert data into the database.
            TaskTb taskTb = new TaskTb()
            {
                TaskId = TaskDTO.TaskId,
                Description = TaskDTO.Description,
                UserId = TaskDTO.UserId,
                DueDate = (DateTime)TaskDTO.DueDate,
                Priority = TaskDTO.Priority,
                ProjectId = TaskDTO.ProjectId,
                Status = TaskDTO.Status,
                Title = TaskDTO.Title
            };

            try
            {
                var assignedUser = await _context.UserTb.FindAsync(TaskDTO.UserId);
                if (assignedUser == null)
                {
                    // Return 404 if the user does not exist
                    return NotFound("Assigned user does not exist.");
                }
                _logger.LogInformation("Creating task with Title: {Title} and TaskId: {TaskId}", taskTb.Title, taskTb.TaskId);

                // Add the new project entity to the ProjectTb table in the database context.
                _context.TaskTb.Add(taskTb);

                // This will insert the new record if the operation is successful.
                await _context.SaveChangesAsync();


                // It returns the created project entity
                return CreatedAtAction("GetTaskById", new { id = taskTb.TaskId }, taskTb);
                //GetProjectTb is there instaed of GetProjectById
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                // It throws an internal server error if the passing data is invalid
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
        [HttpPost("{taskId}")]
        public async Task<ActionResult<TaskTb>> CreateTask(int taskId, TaskDTO taskDTO)
        {
            // Check if a task with the specified TaskId already exists
            var existingTask = await _context.TaskTb.FindAsync(taskId);
            if (existingTask != null)
            {
                // Return 409 Conflict if the task with the specified TaskId already exists
                return Conflict("A task with this TaskId already exists.");
            }

            // Verify if the assigned user exists in the database
            var assignedUser = await _context.UserTb.FindAsync(taskDTO.UserId);
            if (assignedUser == null)
            {
                // Return 404 Not Found if the user does not exist
                return NotFound("Assigned user does not exist.");
            }

            // Initialize a new TaskTb object with the provided TaskDTO properties
            TaskTb taskTb = new TaskTb()
            {
                TaskId = taskId,  // Use the specified TaskId from the URL
                Description = taskDTO.Description,
                UserId = taskDTO.UserId,
                DueDate =(DateTime)taskDTO.DueDate,
                Priority = taskDTO.Priority,
                ProjectId = taskDTO.ProjectId,
                Status = taskDTO.Status,
                Title = taskDTO.Title
            };

            try
            {
                _logger.LogInformation("Creating task with Title: {Title} and TaskId: {TaskId}", taskTb.Title, taskTb.TaskId);

                // Add the new task entity to the TaskTb table in the database context
                _context.TaskTb.Add(taskTb);

                // Insert the new record into the database
                await _context.SaveChangesAsync();

                // Return the created task entity with a 201 Created status and the location of the new resource
                return CreatedAtAction("GetTaskById", new { id = taskTb.TaskId }, taskTb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Return a 500 Internal Server Error in case of any exceptions
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Deletes a task by its ID from the TaskTb table.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <returns>NoContent if the deletion is successful, or an error message if something goes wrong.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                // Attempt to find the project by ID
                var taskTb = await _context.TaskTb.FindAsync(id);
                _logger.LogInformation("Deleting task with TaskId: {TaskId}", id);
                // Return 404 if the project was not found
                if (taskTb == null)
                {
                    return NotFound("The task with the specified ID was not found.");
                }

                // Removing the project from the database
                _context.TaskTb.Remove(taskTb);
                await _context.SaveChangesAsync();

                // Return 204(No Content) if the deletion is successful
                return NoContent();
            }
            //
            catch (Exception ex)
            {

                // Return a 500 Internal Server Error if something goes wrong
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while attempting to delete the project.");
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
        //    await _context.SaveChangesAsync();

        //    return NoContent(); // 204 No Content response if successful
        //}
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

            task.Status = request.Status;
            task.Priority = request.Priority; // Update priority
            task.DueDate = (DateTime)request.DueDate; // Update due date

            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content response if successful
        }
        [HttpGet("groupedByProject")]
        public async Task<IActionResult> GetTasksGroupedByProject()
        {
            var groupedTasks = await _context.TaskTb
                .GroupBy(t => t.ProjectId)
                .Select(group => new
                {
                    ProjectId = group.Key,
                    Tasks = group.Select(task => new
                    {
                        task.TaskId,
                        task.Title,
                        task.Description,
                        task.DueDate,
                        task.Priority,
                        task.Status,
                        task.UserId
                    }).ToList()
                })
                .ToListAsync();

            return Ok(groupedTasks);
        }

    }
}
