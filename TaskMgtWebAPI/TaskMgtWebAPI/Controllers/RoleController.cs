using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskMgtWebAPI.DTOS;
using TaskMgtWebAPI.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ActionResult<IEnumerable<RoleTb>>> GetRoles()
        {

        }
        public async Task<ActionResult<RoleTb>> GetRoleById(int id)
        {

        }
        public async Task<ActionResult<ProjectTb>> CreateRole(RoleDTO roleDTO)
        {

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

        private bool RoleTbExists(int id)
        {
            return _context.RoleTb.Any(e => e.RoleId == id);
        }
        public async Task<IActionResult> DeleteRole(int id)
        {

        }
    }
}
