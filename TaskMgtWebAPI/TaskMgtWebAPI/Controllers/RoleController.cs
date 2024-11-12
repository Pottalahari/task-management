using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgtWebAPI.DTOS;
using TaskMgtWebAPI.Models;
namespace TaskMgtWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly TaskMgtDBContext _context;
        //property for logger
        private readonly ILogger<RoleController> _logger;
        public RoleController(TaskMgtDBContext context, ILogger<RoleController> logger)

        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<RoleTb>> GetRole()
        {
            _logger.LogInformation("Data is retrive successfully");
            try
            {
                var role = await _context.RoleTb.ToListAsync();
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occured: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTb>> GetRoleById(int id)
        {
            _logger.LogInformation("Data is GetById successfully");
            // it is retrive the data from database by Id
            try
            {
                var RoleTb = await _context.ProjectTb.FindAsync(id);

                if (RoleTb == null)
                {
                    return NotFound();
                }
                return Ok(RoleTb);

            }
            catch (Exception ex)
            {
                return BadRequest($"an error is occured: {ex.Message}");
            }
        }
        [HttpPost]
        //Method to create a new RoleTb record in the database
        public async Task<ActionResult<RoleTb>> CreateRole(RoleDTO roleDTO)
        {
            _logger.LogInformation("Data is created successfully");
            // This object will store the properties provided in the RoleDTO and be used to insert data into the database.
            RoleTb role = new RoleTb()
            {
                RoleId = roleDTO.RoleId,
                RoleName = roleDTO.RoleName
            };
            _context.RoleTb.Add(role);
            try
            {
                // This will insert the new record if the operation is successful.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RoleTbExists(role.RoleId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetRole", new { id = role.RoleId }, role);
        }
        private bool RoleTbExists(int role)
        {
            return _context.RoleTb.Any(e => e.RoleId == role);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleDTO roleDTO)
        {
            //It checks the given Id is exists in the database or not
            if (id != roleDTO.RoleId)
            {
                return BadRequest();
            }

            RoleTb roleTb = new RoleTb()
            {
                RoleId = roleDTO.RoleId,
                RoleName = roleDTO.RoleName
            };

            //It makes the role table entity is modified in the context
            _context.Entry(roleTb).State = EntityState.Modified;

            try
            {
                _logger.LogInformation("Data is updated successfully");
                // It saves the changes in the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleTbExists(id))
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                // Attempt to find the project by ID
                var roleTb = await _context.RoleTb.FindAsync(id);

                // Return 404 if the project was not found
                if (roleTb == null)
                {
                    return NotFound("The project with the specified ID was not found.");
                }

                // Removing the project from the database
                _context.RoleTb.Remove(roleTb);
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

