using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using nFact.Engine;
using nFact.controllers;
using nFact.viewModels;

namespace nFact.modules
{
    public class IndexModule : NancyModule
    {
        readonly CommandController _controller = new CommandController();

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
            var project = _controller.GetProject(spec);
            if (project == null)
                throw new ApplicationException(string.Format("Could not find project '{0}'", spec));

            var result = new Project {Name = project.Name};
            var environments = new List<Environment>();
            foreach (var e in project.TestEnvironments.Values)
            {
                var environment = new Environment {Name = e.Name};
                var allStoryResults = new List<StoryResult>();
                foreach (var a in e.Artifacts)
                {
                    var storyResults = a.GetStoryResults();
                    foreach (var r in storyResults)
                    {
                        var sr = new StoryResult
                                     {
                                         Description = r.Description,
                                         DurationSecs = r.Seconds,
                                         Id = r.Id,
                                         Name = r.Name,
                                         Result = r.Result.ToString(),
                                         TestRun = a.TestRun,
                                         TestTime = a.Date,
                                         TestVersion = a.Version
                                     };
                        allStoryResults.Add(sr);
                    }
                }
                environment.StoryResults = allStoryResults.ToArray();
                environments.Add(environment);
            }
            result.Environments = environments.ToArray();
            
            if (format.Equals("json", StringComparison.CurrentCultureIgnoreCase))
                return Response.AsJson(result);

            return HttpStatusCode.BadRequest;
        }

        private dynamic SaveSettings()
        {
            PageDataModelProxy data = this.Bind();
            _controller.UpdatePageDataModel(data.Instance());

            return HttpStatusCode.OK;
        }
    }

    public class Project
    {
        public string Name;
        public Environment[] Environments;
    }

    public class Environment
    {
        public string Name;
        public StoryResult[] StoryResults;
    }

    public class StoryResult
    {
        public int TestRun;
        public DateTime TestTime;
        public string TestVersion;
        public string Name;
        public string Description;
        public string Result;
        public double DurationSecs;
        public string Id;
    }
}