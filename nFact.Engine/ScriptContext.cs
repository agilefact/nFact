using System.Collections;
using System.IO;
using ScriptRunner;
using nFact.Engine.Model;
using nFact.Shared;

namespace nFact.Engine
{
    public class ScriptContext : IScriptScenarioContext
    {
        public Hashtable Parameters { get; private set; }
        public int ScenarioCount { get; set; }
        public string Script { get; private set; }
        public IArtifacts Artifacts { get; private set; }

        public ScriptContext(IScript model, ProjectArtifacts artifacts)
        {
            Script = model.Script;
            Artifacts = artifacts;
            Directory.CreateDirectory(artifacts.FilePath);
            Parameters = new Hashtable
                                 {
                                     {"RootPath", model.RootPath},
                                     {"SpecName", model.Spec},
                                     {"ArtifactsPath", artifacts.FilePath},
                                     {"TestRun", artifacts.TestRun},
                                     {"Environment", model.Environment},
                                     {"Version", artifacts.Version},
                                 };
        }
    }
}