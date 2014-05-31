using Moq;
using NUnit.Framework;
using nFact.Engine.Commands;
using nFact.Engine.Model;
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

            var context = new CommandContext(scriptModel, scriptContext.Object);
            context.VideoRecorder = recorder.Object;


            var handler = new CommandHandler(context);

            handler.Parse("{Scenario.Start()}");
            recorder.Verify(r => r.Start(It.IsAny<IScriptScenarioContext>()));

            handler.Parse("{Scenario.End()}");
            recorder.Verify(r => r.End(It.IsAny<IScriptScenarioContext>()));
        }

    }
}
