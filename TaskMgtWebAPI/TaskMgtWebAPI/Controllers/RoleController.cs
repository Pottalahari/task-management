using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<ActionResult<IEnumerable<RoleTb>>> GetRoles()
        {

        }
        public async Task<ActionResult<RoleTb>> GetRoleById(int id)
        {

        }
        public async Task<ActionResult<ProjectTb>> CreateRole(RoleDTO roleDTO)
        {

        }
        public async Task<IActionResult> UpdateRole(int id, RoleDTO roleDTO)
        {

        }
        public async Task<IActionResult> DeleteRole(int id)
        {

        }
    }
}
