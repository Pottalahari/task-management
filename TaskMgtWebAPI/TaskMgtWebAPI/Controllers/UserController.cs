using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // Property for logger
        private readonly ILogger<UserController> _logger;

        public UserController(TaskMgtDBContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all users from the UserTb table.
        /// Logs the request and handles any exceptions that might occur.
        /// </summary>
        /// <returns>A list of users from the UserTb table.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTb>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Fetching all users from the database");
                // Retrieve all users from the UserTb table asynchronously.
                var users = await _context.UserTb.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Return a bad request response with an error message if an exception occurs.
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a user by their UserId from the UserTb table.
        /// Returns a NotFound response if the user doesn't exist, and a BadRequest for errors.
        /// </summary>
        /// <param name="id">The UserId of the user to retrieve.</param>
        /// <returns>The user with the specified UserId, or a NotFound response.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserTb>> GetUserById(int id)
        {
            try
            {
                // Find the user by its ID asynchronously.
                var user = await _context.UserTb.FindAsync(id);

                // If the user is not found, return a NotFound response.
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing user with the provided UserDTO.
        /// Returns NoContent on success, BadRequest if IDs don't match, and handles concurrency exceptions.
        /// </summary>
        /// <param name="id">The UserId of the user to update.</param>
        /// <param name="userDTO">The user data transfer object containing updated details.</param>
        /// <returns>NoContent if the update is successful, or an error message if it fails.</returns>
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

        /// <summary>
        /// Creates a new user in the UserTb table using the provided UserDTO.
        /// Returns CreatedAtAction with the newly created user, or Conflict if the user already exists.
        /// </summary>
        /// <param name="userDTO">The user data transfer object containing the user details.</param>
        /// <returns>The newly created user if successful.</returns>
        [HttpPost]
        public async Task<ActionResult<UserTb>> CreateUser(UserDTO userDTO)
        {
            _logger.LogInformation("Attempting to create a new user with UserId: {UserId}.", userDTO.UserId);

            // This object will store the properties provided in the UserDTO and be used to insert data into the database.
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
            return CreatedAtAction("GetUserById", new { id = userTb.UserId }, userTb);
        }

        /// <summary>
        /// Deletes a user by their UserId from the UserTb table.
        /// Returns NoContent on success or NotFound if the user doesn't exist.
        /// </summary>
        /// <param name="id">The UserId of the user to delete.</param>
        /// <returns>NoContent if the deletion is successful, or an error message if it fails.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Attempt to find the user by ID
                var userTb = await _context.UserTb.FindAsync(id);

                // Return 404 if the user was not found
                if (userTb == null)
                {
                    return NotFound("The user with the specified ID was not found.");
                }

                // Removing the user from the database
                _context.UserTb.Remove(userTb);
                await _context.SaveChangesAsync();

                // Return 204(No Content) if the deletion is successful
                return NoContent();
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error if something goes wrong
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while attempting to delete the user.");
            }
        }
    }
}