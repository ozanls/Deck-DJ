using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DeckDJ.Startup))]
namespace DeckDJ
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
