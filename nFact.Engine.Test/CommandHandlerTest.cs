using Moq;
using NUnit.Framework;
using nFact.Engine.Commands;
using nFact.Engine.Model;
using nFact.Media;
using nFact.Shared;

namespace nFact.Engine.Test
{
    [TestFixture]
    public class CommandHandlerTest
    {
        [Test]
        public void ParseScenarioStartEnd()
        {
            var recorder = new Mock<IRecorder>();
            var scriptModel = new ScriptEngineModel();
            scriptModel.RecordVideo = true;
            var scriptContext = new Mock<IScriptScenarioContext>();
            
            recorder.Setup(r => r.Start(scriptContext.Object)).Verifiable();

            var recorders = new Recorders();
            recorders.VideoRecorder = recorder.Object;
            var context = new CommandContext(scriptModel, scriptContext.Object, recorders);


            var handler = new CommandHandler(context);

            handler.Parse("{Scenario.Start(1)}");
            recorder.Verify(r => r.Start(It.IsAny<IScriptScenarioContext>()));

            scriptContext.VerifySet(c => c.ScenarioCount = 1);

            handler.Parse("{Scenario.End()}");
            recorder.Verify(r => r.End(It.IsAny<IScriptScenarioContext>()));
        }

    }
}
