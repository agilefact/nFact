using System.IO;
using System.Reflection;
using System.Web.Http;
using Owin;
using nFact.Engine;

namespace nFact
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ScriptEngine.Instance.Init();

            // Configure Static Files for self-host. 
            var exeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Configure Web API for self-host. 
            var webApiConfig = new HttpConfiguration();
            webApiConfig.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{spec}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseNancy();
        } 
    }
}