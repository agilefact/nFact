

using ScriptRunner;

namespace nFact.Shared
{
    public interface IScriptScenarioContext : IScriptContext
    {
        int ScenarioCount { get; set; }
    }
}