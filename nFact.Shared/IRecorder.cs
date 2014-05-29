using ScriptRunner;

namespace nFact.Shared
{
    public interface IRecorder
    {
        void Start(IScriptScenarioContext context);
        void End(IScriptScenarioContext context);
        void Dispose();
    }
}