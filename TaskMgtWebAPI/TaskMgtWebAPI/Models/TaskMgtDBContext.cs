using Microsoft.EntityFrameworkCore;
namespace TaskMgtWebAPI.Models
{
    public class TaskMgtDBContext : DbContext
    {
        public TaskMgtDBContext()
        {

        }
        public TaskMgtDBContext(DbContextOptions<TaskMgtDBContext> options) : base(options) { }

        public virtual DbSet<UserTb> UserTb { get; set; }
        public virtual DbSet<TaskTb> TaskTb { get; set; }
        public virtual DbSet<ProjectTb> ProjectTb { get; set; }
        public virtual DbSet<RoleTb> RoleTb { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.

       => optionsBuilder.UseSqlServer("Data Source=CHINNULAHARI\\SQLEXPRESS;Initial Catalog=TaskMgtDB;Integrated Security=True;TrustServerCertificate=True");

       => optionsBuilder.UseSqlServer("Data Source=Maha\\SQLEXPRESS;Initial Catalog=TaskMgtDB;Integrated Security=True;TrustServerCertificate=True");


    }
}
