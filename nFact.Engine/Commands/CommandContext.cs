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

        public CommandContext(ScriptEngineModel model, IScriptScenarioContext context, Recorders recorders)
        {
            Model = model;
            ScriptContext = context;
            VideoRecorder = recorders.VideoRecorder;
            SlidesRecorder = recorders.SlidesRecorder;
        }
    }
}