using Moq;
using NUnit.Framework;
using nFact.Engine.Model;

namespace nFact.Engine.Test
{
    [TestFixture]
    public class StoryManagerTest
    {
        [Test]
        public void GetProjectStoryResultsTest()
        {
            var project = new Project("SpecTests");
            var a1 = project.CreateArtifacts("local", "1.0.0");
            a1.RecordTestComplete();

            //project.CreateArtifacts("UAT", "1.0.1");
            //project.CreateArtifacts("local", "1.0.2");
            //project.CreateArtifacts("UAT", "1.0.3");

            var manager = new StoryManager();
            var results = manager.GetProjectStoryResults(project);
            Assert.AreEqual("local", results.Environments[0].Name);
        }
    }
}