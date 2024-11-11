using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Correct namespace for EF Core
using TaskMgtWebAPI.DTOS;
using TaskMgtWebAPI.Models;
using Microsoft.Extensions.Logging;
using System;
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


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskTb>>> GetTasks()
        {
            _logger.LogInformation("");

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
                    return NotFound();
                }
                return Ok(TaskTb);

            }
            catch (Exception ex)
            {
                return BadRequest($"an error is occured: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskDTO TaskDTO)
        {
            if (id != TaskDTO.UserId)
            {
                return BadRequest();
            }

            TaskTb taskTb = new TaskTb()
            {
                 TaskId=TaskDTO.TaskId,
                 Description=TaskDTO.Description,
                 UserId=TaskDTO.UserId,
                 DueDate=TaskDTO.DueDate,
                 Priority=TaskDTO.Priority,
                 ProjectId=TaskDTO.ProjectId,
                 Status=TaskDTO.Status,
                 Title=TaskDTO.Title
            };


            _context.Entry(taskTb).State = EntityState.Modified;
            try
            {
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
                DueDate = TaskDTO.DueDate,
                Priority = TaskDTO.Priority,
                ProjectId = TaskDTO.ProjectId,
                Status = TaskDTO.Status,
                Title = TaskDTO.Title
            };

            try
            {
                _logger.LogInformation(" created a request as per the id ");
                // Add the new project entity to the ProjectTb table in the database context.
                _context.TaskTb.Add(taskTb);

                // This will insert the new record if the operation is successful.
                await _context.SaveChangesAsync();


                // It returns the created project entity
                return CreatedAtAction("GetTasks", new { id = taskTb.TaskId }, taskTb);
                //GetProjectTb is there instaed of GetProjectById
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                // It throws an internal server error if the passing data is invalid
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                // Attempt to find the project by ID
                var taskTb = await _context.TaskTb.FindAsync(id);

                // Return 404 if the project was not found
                if (taskTb == null)
                {
                    return NotFound("The project with the specified ID was not found.");
                }

                // Removing the project from the database
                _context.TaskTb.Remove(taskTb);
                await _context.SaveChangesAsync();

                // Return 204(No Content) if the deletion is successful
                return NoContent();
            }
            catch (Exception ex)
            {

                // Return a 500 Internal Server Error if something goes wrong
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while attempting to delete the project.");
            }
        }
    }
}
