using System;
using Nancy;
using nFact.controllers;
using nFact.viewModels;

namespace nFact.modules
{
    public class ChartModule : NancyModule
    {
        readonly ChartController _controller = new ChartController();

        public ChartModule()
        {
            Get["/{spec}/deployment"] = p => BuildProjectDeploymentChart(p.spec);
            Get["/{spec}/story-cycle-time"] = p => BuildProjectCycleChart(p.spec);
            Get["/{spec}/story/{id}/chart"] = p => BuildStoryChart(p.spec, p.id);
            Get["/{spec}/story/{id}/cycle"] = p => GetStoryCycleData(p.spec, p.id);
        }

        private dynamic BuildProjectCycleChart(string spec)
        {
            string format = Request.Query.format;
            if (format == null)
            {
                var vm = BuildViewModel(spec, null);
                return View["charts/storyCycle", vm];
            }

            return GetProjectData(spec);
        }

        private dynamic BuildProjectDeploymentChart(string spec)
        {
            string format = Request.Query.format;
            if (format == null)
            {
                var vm = BuildViewModel(spec, null);
                return View["charts/projectDeployment", vm];
            }

            return GetProjectData(spec);
        }

        private dynamic BuildStoryChart(string spec, string storyId)
        {
            string format = Request.Query.format;
            if (format == null)
            {
                var vm = BuildViewModel(spec, storyId);
                return View["charts/story", vm];
            }

            return GetStoryData(spec, storyId);
        }

        private ChartViewModel BuildViewModel(string spec, string storyId)
        {
            var data = new ChartDataModel
                                {
                                    spec = spec,
                                    storyId = storyId
                                };

            return new ChartViewModel(data);
        }

        private dynamic GetProjectData(string spec)
        {
            string format = Request.Query.format;
            var result = _controller.GetDeploymentCycleTime(spec);

            return ResultResponse(format, result);
        }

        private dynamic GetStoryData(string spec, string id)
        {
            string format = Request.Query.format;
            var result = _controller.GetStoryChartData(spec, id);

            return ResultResponse(format, result);
        }

        private dynamic GetStoryCycleData(string spec, string id)
        {
            string format = Request.Query.format;
            var result = _controller.GetDeploymentCycleTime(spec, id);

            return ResultResponse(format, result);
        }

        private dynamic ResultResponse<T>(string format, T result)
        {
            if (string.IsNullOrEmpty(format))
                return HttpStatusCode.BadRequest;

            if (format.Equals("json", StringComparison.CurrentCultureIgnoreCase))
                return base.Response.AsJson(result);

            if (format.Equals("xml", StringComparison.CurrentCultureIgnoreCase))
                return base.Response.AsXml(result);

            return HttpStatusCode.BadRequest;
        }
    }
}
