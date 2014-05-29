using System;
using Nancy;
using Nancy.Responses.Negotiation;
using nFact.controllers;
using nFact.viewModels;

namespace nFact.modules
{
    public class SourceModule : NancyModule
    {
        readonly SourceController _controller = new SourceController();

        public SourceModule()
        {
            Get["/{spec}/latest"] = p => GetView(p.spec);
        }

        private Negotiator GetView(string spec)
        {
            if (string.IsNullOrEmpty(spec))
                throw new ApplicationException("spec must be specified in '/{spec}/latest' url");

            var tfsSourcePath = _controller.GetTfsSourcePath(spec);
            var sourceViewModel = new SourceViewModel(tfsSourcePath);
            
            return View["source", sourceViewModel];
        }
    }
}
