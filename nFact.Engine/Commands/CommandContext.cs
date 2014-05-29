using nFact.Engine.Model;
using nFact.Media;
using nFact.Shared;

namespace nFact.Engine.Commands
{
    public class CommandContext
    {
        public IScriptScenarioContext ScriptContext { get; set; }
        public ScriptEngineModel Model { get; set; }
        public IRecorder VideoRecorder { get; set; }
        public IRecorder SlidesRecorder { get; set; }

        public CommandContext(ScriptEngineModel model, IScriptScenarioContext context)
        {
            Model = model;
            ScriptContext = context;
            VideoRecorder = new VideoRecorder();
            SlidesRecorder = new SlidesRecorder();
        }
    }
}