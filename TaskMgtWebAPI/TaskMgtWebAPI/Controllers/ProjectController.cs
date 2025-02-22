using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TaskMgtWebAPI.Models;
using TaskMgtWebAPI.DTOS;



namespace TaskMgtWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly TaskMgtDBContext _context;

        //property for logger
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(TaskMgtDBContext context, ILogger<ProjectController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of all projects from the database
        /// 
        /// </summary>
        /// <returns>Returns a list of projects as an HTTP 200 OK response on success.</returns>    
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTb>>> GetProjects()
        {
            _logger.LogInformation("");

            try
            {
                _logger.LogInformation(" Recevied a get request");
                // Retrieve all projects from the ProjectTb table asynchronously.
                var Project = await _context.ProjectTb.ToListAsync();
                return Ok(Project);
            }
            catch (Exception ex)
            {
                // Return a bad request response with an error message if an exception occurs.
                return BadRequest($"An error occured: {ex.Message}");
            }

        }

        /// <summary>
        /// Retrieves the product by specific ID
        /// </summary>
        /// <param name="id">the ID of the project to retrieve</param>
        /// <returns>Returns the project with the specified ID if found; otherwise, returns a 404 Not Found.</returns>
        [HttpGet("{id}")]
        
        public async Task<ActionResult<ProjectTb>> GetProjectById(int id)
        {
            // it is retrive the data from database by Id
            try
            {
                // Find the project by its ID asynchronously.
                var ProjectTb = await _context.ProjectTb.FindAsync(id);

                // If the project is not found, return a NotFound response.
                if (ProjectTb == null)
                {
                    return NotFound();
                }
                return Ok(ProjectTb);

            }
            catch (Exception ex)
            {
                return BadRequest($"an error is occured: {ex.Message}");
            }
        }

        /// <summary>
        ///  Updates an existing project with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the project to update</param>
        /// <param name="projecttbdto">The updated project data</param>
        /// <returns>A status indicating the result of the update operation</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, ProjectDTO projectDTO)
        {
            if (id != projectDTO.ProjectId)
            {
                return BadRequest();
            }

            ProjectTb projectTb = new ProjectTb()
            {
                ProjectId = projectDTO.ProjectId,
                ProjectName = projectDTO.ProjectName,
                StartDate = projectDTO.StartDate,
                EndDate = projectDTO.EndDate,
                Description = projectDTO.Description

            };


            _context.Entry(projectTb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectTbExists(id))
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
        private bool ProjectTbExists(int id)
        {
            return _context.ProjectTb.Any(e => e.ProjectId == id);
        }


        /// <summary>
        /// Creates a new project and adds it to the database
        ///
        /// </summary>
        /// <param name="projectDTO">The project data to be added</param>
        /// <returns>
        /// Returns the created project with a 201 Created status on success,
        /// or a 400 Bad Request if the data is invalid
        /// </returns>
        [HttpPost]
        //Method to create a new ProjectTb record in the database
        public async Task<ActionResult<ProjectTb>> CreateProject(ProjectDTO projectDTO)
        {
            // This object will store the properties provided in the ProjectDTO and be used to insert data into the database.
            ProjectTb projectTb = new ProjectTb()
            {
                // Setting the ProjectId property of projectTb to the ProjectId value from the projectDTO.
                ProjectId = projectDTO.ProjectId,
                ProjectName = projectDTO.ProjectName,
                StartDate = projectDTO.StartDate,
                EndDate = projectDTO.EndDate,
                Description = projectDTO.Description
            };

            try
            {
                _logger.LogInformation(" created a request as per the id ");
                // Add the new project entity to the ProjectTb table in the database context.
                _context.ProjectTb.Add(projectTb);

                // This will insert the new record if the operation is successful.
                await _context.SaveChangesAsync();


                // It returns the created project entity
                return CreatedAtAction("GetProjectById", new { id = projectTb.ProjectId }, projectTb);
                //GetProjectTb is there instaed of GetProjectById
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                // It throws an internal server error if the passing data is invalid
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        /// <summary>
        /// Deletes a project by its unique ID
        /// </summary>
        /// <param name="id">The ID of the project to delete</param>
        /// <returns>
        /// A status code indicating the result of the delete operation
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                // Attempt to find the project by ID
                var projectTb = await _context.ProjectTb.FindAsync(id);

                // Return 404 if the project was not found
                if (projectTb == null)
                {
                    return NotFound("The project with the specified ID was not found.");
                }

                // Removing the project from the database
                _context.ProjectTb.Remove(projectTb);
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
