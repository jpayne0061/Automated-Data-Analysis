using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CorrelationStation.Startup))]
namespace CorrelationStation
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
