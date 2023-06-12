using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TFI.Startup))]
namespace TFI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
