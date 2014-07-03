using System;
using System.IO;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using nFact.Engine.Model.DataTransfer;
using nFact.controllers;
using nFact.viewModels;

namespace nFact.modules
{
    public class IndexModule : NancyModule
    {
        readonly IndexController _controller = new IndexController();
        readonly IndexDtoController _dtoController = new IndexDtoController();

        public IndexModule()
        {
            Get["/"] = p => GetResults(null, null);
            Get["/{spec}"] = p => GetResults(p.spec, null);
            //Get["/{spec}/{test}"] = p => GetResults(p.spec, p.test, true);
            Get["/{spec}/{test}/artifacts/{file}"] = p => GetFile(p.spec, p.test, p.file);
            Get["/{spec}/story/{id}"] = p => GetCurrentResultsByStory(p.spec, p.id);
            Get["/{spec}/story/{id}/history"] = p => GetResultsByStory(p.spec, p.id);
            Get["/{spec}/story"] = p => GetCurrentResultsByStory(p.spec, null);
            Post["/{spec}/story/{id}/test/{test}/accept"] = p => Accept(p.spec, p.id, p.test);
            Post["/settings"] = p => SaveSettings();
        }

        private dynamic Accept(string spec, string storyId, string test)
        {
            var testRun = int.Parse(test);
            var result = _dtoController.AcceptStory(spec, storyId, testRun);
            return result ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        }

        private dynamic GetResults(string spec, string test, bool showControls = false)
        {
            string format = Request.Query.format;
            if (format != null)
                return GetResultsByProject(spec, format);

            return View["index", BuildViewModel(spec, test, showControls)];
        }

        private dynamic GetFile(string spec, string test, string file)
        {
            var artifacts = _controller.GetArtifacts(spec, test);
            if (artifacts == null)
                return HttpStatusCode.NotFound;

            var filePath = Path.Combine(artifacts.FilePath, file);

            var mimeType = MimeTypes.GetMimeType(file);
            var response = new StreamResponse(() => new FileStream(filePath, FileMode.Open), mimeType);

            if (mimeType == "text/plain")
                return base.Response.AsText(File.ReadAllText(filePath));

            return response.AsAttachment(file);
        }

        private IndexViewModel BuildViewModel(string spec, string test, bool showControls = false)
        {

            var dataModel = PageDataModelBuilder.Build(spec);
            var viewModel = new IndexViewModel(dataModel);

            spec = dataModel.selectedSpec;
            if (spec == null)
                return viewModel; // no projects available
            
            var artifacts = _controller.GetArtifacts(spec, test);
            if (artifacts == null)
                return viewModel; // no results available

            var testRun = artifacts.TestRun;
            dataModel = PageDataModelBuilder.Build(spec, testRun);
            viewModel = new IndexViewModel(dataModel);

            viewModel.RenderReport(artifacts);
            viewModel.Controls.ControlsVisible = showControls;

            return viewModel;
        }

        private dynamic GetResultsByProject(string spec, string format)
        {
            var result = _dtoController.GetResultsByEnvironmentStory(spec);

            return ResultResponse(format, result);
        }

        private dynamic GetResultsByStory(string spec, string id)
        {
            string format = Request.Query.format;
            var result = _dtoController.GetResultsByEnvironmentStory(spec, id);

            return ResultResponse(format, result);
        }

        private dynamic GetCurrentResultsByStory(string spec, string id)
        {
            string format = Request.Query.format;
            var result = _dtoController.GetCurrentProjectStoryResults(spec, id);

            return ResultResponse(format, result);
        }

        private dynamic ResultResponse(string format, Project result)
        {
            if (string.IsNullOrEmpty(format))
                return HttpStatusCode.BadRequest;

            if (format.Equals("json", StringComparison.CurrentCultureIgnoreCase))
                return base.Response.AsJson(result);

            if (format.Equals("xml", StringComparison.CurrentCultureIgnoreCase))
                return base.Response.AsXml(result);

            return HttpStatusCode.BadRequest;
        }

        private dynamic SaveSettings()
        {
            PageDataModelProxy data = this.Bind();
            _controller.UpdatePageDataModel(data.Instance());

            return HttpStatusCode.OK;
        }
    }
}