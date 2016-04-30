using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzizaHadoopStatistics.Startup))]
namespace AzizaHadoopStatistics
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
