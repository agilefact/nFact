using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using nFact.DataTransfer;
using nFact.Engine;
using nFact.controllers;
using nFact.viewModels;
using Environment = nFact.DataTransfer.Environment;

namespace nFact.modules
{
    public class IndexModule : NancyModule
    {
        readonly CommandController _controller = new CommandController();
        readonly DataTransferController _dtoController = new DataTransferController();

        public IndexModule()
        {
            Get["/"] = p => GetResults(null, null);
            Get["/{spec}"] = p => GetResults(p.spec, null);
            Get["/{spec}/{test}"] = p => GetResults(p.spec, p.test, true);
            Get["/{spec}/{test}/artifacts/{file}"] = p => GetFile(p.spec, p.test, p.file);
            Post["/settings"] = p => SaveSettings();
        }

        private dynamic GetResults(string spec, string test, bool showControls = false)
        {
            string format = Request.Query.format;
            if (format != null)
                return GetAllResults(spec, format);

            return View["index", BuildViewModel(spec, null)];
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
                return Response.AsText(File.ReadAllText(filePath));

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

            viewModel.RenderReport(artifacts);
            viewModel.Controls.ControlsVisible = showControls;

            return viewModel;
        }

        private dynamic GetAllResults(string spec, string format)
        {
            var result = _dtoController.GetProjectStoryResults(spec);

            if (format.Equals("json", StringComparison.CurrentCultureIgnoreCase))
                return Response.AsJson(result);

            if (format.Equals("xml", StringComparison.CurrentCultureIgnoreCase))
                return Response.AsXml(result);

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