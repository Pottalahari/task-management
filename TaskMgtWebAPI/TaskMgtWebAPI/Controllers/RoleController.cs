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
        public RoleController(TaskMgtDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<RoleTb>> GetRole()
        {
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
    }
}

