using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Eburger.Startup))]
namespace Eburger
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
