using nFact.Engine;
using nFact.Engine.Configuration;

namespace nFact.controllers
{
    class RunnerController
    {
        public void RunSpec(IScript model, string environment)
        {
            var manager = new ProjectConfiguratonManager(model.Spec);
            manager.SetEnvironment(environment);

            ScriptEngine.Instance.Start(model, environment);
        }
    }
}
