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
    public class UserController : ControllerBase
    {
        private readonly TaskMgtDBContext _context;

        //property for logger
        private readonly ILogger<UserController> _logger;

        public UserController(TaskMgtDBContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTb>>> GetUsers()
        {
            _logger.LogInformation("");

            try
            {
                _logger.LogInformation(" Recevied a get request");
                // Retrieve all projects from the ProjectTb table asynchronously.

                var user = await _context.UserTb.ToListAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                // Return a bad request response with an error message if an exception occurs.
                return BadRequest($"An error occured: {ex.Message}");
            }
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<UserTb>> GetUserById(int id)
        {
            // it is retrive the data from database by Id
            try
            {
                // Find the project by its ID asynchronously.
                var UserTb = await _context.UserTb.FindAsync(id);

                // If the project is not found, return a NotFound response.
                if (UserTb == null)
                {
                    return NotFound();
                }
                return Ok(UserTb);

            }
            catch (Exception ex)
            {
                return BadRequest($"an error is occured: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDTO userDTO)
        {
            if (id != userDTO.UserId)
            {
                return BadRequest();
            }

            UserTb userTb = new UserTb()
            {
                UserId = userDTO.UserId,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                Password = userDTO.Password,
                RoleId = userDTO.RoleId

            };
            _context.Entry(userTb).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserTbExists(id))
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
        private bool UserTbExists(int id)
        {
            return _context.UserTb.Any(e => e.UserId == id);
        }
        [HttpPost]
        //Method to create a new RoleTb record in the database
        public async Task<ActionResult<UserTb>> CreateRole(UserDTO userDTO)
        {
            _logger.LogInformation("Data is created successfully");
            // This object will store the properties provided in the RoleDTO and be used to insert data into the database.
            UserTb userTb = new UserTb()
            {
                UserId = userDTO.UserId,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                Password = userDTO.Password,
                RoleId = userDTO.RoleId
            };
            _context.UserTb.Add(userTb);
            try
            {
                // This will insert the new record if the operation is successful.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserTbExists(userTb.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetUsers", new { id = userTb.UserId }, userTb);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Attempt to find the project by ID
                var userTb = await _context.UserTb.FindAsync(id);

                // Return 404 if the project was not found
                if (userTb == null)
                {
                    return NotFound("The project with the specified ID was not found.");
                }

                // Removing the project from the database
                _context.UserTb.Remove(userTb);
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
