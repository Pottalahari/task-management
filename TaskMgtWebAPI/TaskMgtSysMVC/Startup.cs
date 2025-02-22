using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TaskMgtSysMVC.Startup))]
namespace TaskMgtSysMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
