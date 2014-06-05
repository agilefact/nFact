using System;
using System.Linq;
using nFact.Engine;
using nFact.Engine.Configuration;
using nFact.Engine.Model;
using nFact.viewModels;

namespace nFact.controllers
{
    public class CommandController
    {
        private string[] _specs;
        private readonly SpecManager _specManager;

        public CommandController()
        {
            _specManager = ScriptEngine.Instance.SpecManager;
        }

        public ScriptEngineModel GetEngineDataModel()
        {
            return ScriptEngine.Instance.GetModel();
        }

        public void UpdatePageDataModel(PageDataModel dataModel)
        {
            var engineSettings = new ScriptEngineModel
                                     {
                                         RecordVideo = dataModel.video,
                                         RecordSteps = dataModel.steps
                                     };
            ScriptEngine.Instance.SetModel(engineSettings);

            var manager = new ProjectConfiguratonManager(dataModel.selectedSpec);
            manager.SetEnvironment(dataModel.selectedEnvironment);
        }

        public void SetSpec(string spec)
        {
            var currentSpec = GetCurrentSpec();

            if (_specs == null)
                GetSpecs();

            if (_specs.Length == 0)
                return;

            if (!string.IsNullOrEmpty(spec))
                currentSpec = spec;

            var match = _specs.FirstOrDefault(s => s == currentSpec);
            if (string.IsNullOrEmpty(match))
                currentSpec = _specs[0];

            _specManager.SetCurrentProject(currentSpec);
        }

        public string GetCurrentSpec()
        {
            var project = _specManager.GetCurrentProject();
            return project == null ? null : project.Name;
        }

        public string[] GetSpecs()
        {
            _specs = _specManager.GetProjectSpecs();
            return _specs;
        }

        public ProjectArtifacts GetArtifacts(string spec, string test)
        {
            if (test != null)
            {
                var testRun = int.Parse(test);
                return _specManager.GetArtifacts(spec, testRun);
            }
            return _specManager.GetLatestArtifacts(spec);
        }

        public ProjectArtifacts[] GetArtifacts(string spec)
        {
            return _specManager.GetAllArtifacts(spec);
        }

        public Project GetProject(string spec)
        {
            var project =  _specManager.GetProject(spec);

            if (project == null)
                throw new ApplicationException(String.Format("Could not find project '{0}'", spec));

            return project;
        }
    }
}
