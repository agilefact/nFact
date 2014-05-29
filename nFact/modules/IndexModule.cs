using System.IO;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using nFact.controllers;
using nFact.viewModels;

namespace nFact.modules
{
    public class IndexModule : NancyModule
    {
        readonly CommandController _controller = new CommandController();

        public IndexModule()
        {
            Get["/"] = p => View["index", BuildViewModel(null, null)];
            Get["/{spec}"] = p => View["index", BuildViewModel(p.spec, null)];
            Get["/{spec}/{test}"] = p => View["index", BuildViewModel(p.spec, p.test)];
            Get["/{spec}/{test}/artifacts/{file}"] = p => GetFile(p.spec, p.test, p.file);
            Post["/settings"] = p => SaveSettings();
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

        private IndexViewModel BuildViewModel(string spec, string test)
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
            return viewModel;
        }

        private dynamic SaveSettings()
        {
            PageDataModelProxy data = this.Bind();
            _controller.UpdatePageDataModel(data.Instance());

            return HttpStatusCode.OK;
        }
    }
}