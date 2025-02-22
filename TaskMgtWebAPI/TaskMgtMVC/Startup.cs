using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TaskMgtMVC.Startup))]
namespace TaskMgtMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
