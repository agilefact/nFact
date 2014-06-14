using Nancy;
using Nancy.Conventions;
using Nancy.SassAndCoffee;
using Nancy.Session;
using SassAndCoffee.Core.Caching;

namespace nFact
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            CookieBasedSessions.Enable(pipelines);
            StaticConfiguration.DisableErrorTraces = false;

            Hooks.Enable(pipelines, new InMemoryCache(), container.Resolve<IRootPathProvider>());
            Jsonp.Enable(pipelines);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            // Add serving of static files behaviour
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("assets", "assets"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("artifacts", "artifacts"));
            base.ConfigureConventions(nancyConventions);
        }
    }
}