using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dchang_blog.Startup))]
namespace Dchang_blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
