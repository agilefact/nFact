using System;
using Nancy;
using nFact.Engine.Model.DataTransfer;
using nFact.controllers;
using nFact.viewModels;

namespace nFact.modules
{
    public class ChartModule : NancyModule
    {
        readonly IndexDtoController _dtoController = new IndexDtoController();

        public ChartModule()
        {
            Get["/{spec}/chart"] = p => View["charts/story", BuildViewModel(p.spec)];
            Get["/{spec}/story/{id}/chart"] = p => GetStoryData(p.spec, p.id);
        }

        private ChartViewModel BuildViewModel(string spec)
        {
            var data = new ChartDataModel
                                {
                                    spec = spec
                                };

            return new ChartViewModel(data);
        }

        private dynamic GetStoryData(string spec, string id)
        {
            string format = Request.Query.format;
            var result = _dtoController.GetStoryChartData(spec, id);

            return ResultResponse(format, result);
        }

        private dynamic ResultResponse(string format, ChartData result)
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
