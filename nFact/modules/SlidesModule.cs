using Nancy;
using Nancy.Responses.Negotiation;
using nFact.controllers;
using nFact.viewModels;

namespace nFact.modules
{
    public class SlidesModule : NancyModule
    {
        CommandController _controller = new CommandController();

        public SlidesModule()
        {
            Get["/{spec}/{test}/{scenario}/slides"] = p => GetView(p.spec, p.test, p.scenario);
        }

        private Negotiator GetView(string spec, string test, string scenario)
        {
            var artifacts = _controller.GetArtifacts(spec, test);
            var images = artifacts.GetSlidesRelativeUrl(scenario);
            var vm = new SlidesViewModel(images);

            return View["slides", vm];
        }
    }
}
