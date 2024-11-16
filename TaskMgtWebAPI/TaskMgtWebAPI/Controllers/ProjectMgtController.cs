//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Data.Entity;

////using Microsoft.EntityFrameworkCore;
//using TaskMgtWebAPI.Models;

//namespace TaskMgtWebAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProjectMgtController : ControllerBase
//    {
//        private readonly TaskMgtDBContext _context;
//        private readonly ILogger<ProjectMgtController> _logger;
//        public ProjectMgtController(TaskMgtDBContext context, ILogger<ProjectMgtController> logger)

//        {
//            _context = context;
//            _logger = logger;
//        }
//        [HttpGet("groupedByProject")]
//        public async Task<IActionResult> GetTasksGroupedByProject()
//        {
//            var groupedTasks = await _context.TaskTb
//                .GroupBy(t => t.ProjectId)
//                .Select(group => new
//                {
//                    ProjectId = group.Key,
//                    Tasks = group.Select(task => new
//                    {
//                        task.TaskId,
//                        task.Title,
//                        task.Description,
//                        task.DueDate,
//                        task.Priority,
//                        task.Status,
//                        task.UserId
//                    }).ToList()
//                })
//                .ToListAsync();

//            return Ok(groupedTasks);
//        }

//    }
//}
