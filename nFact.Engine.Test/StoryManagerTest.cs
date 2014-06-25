using System.Linq;
using Moq;
using NUnit.Framework;
using nFact.Engine.Model;

namespace nFact.Engine.Test
{
    [TestFixture]
    public class StoryManagerTest
    {
        [Test]
        public void AcceptStory()
        {
            var project = new Project("SpecTests");
            var a1 = project.CreateArtifacts("local", "1.0.0");
            var a2 = project.CreateArtifacts("UAT", "1.0.1");
            var a3 = project.CreateArtifacts("local", "1.0.2");
            a1.RecordTestComplete();
            a2.RecordTestComplete();
            a3.RecordTestComplete();

            var manager = new StoryManager();
            var accepted = manager.AcceptStoryResult(project, "12345", 3);

            Assert.IsTrue(accepted);

            var canAccept = manager.CanAcceptStory(project, "12345", 1);

            Assert.IsFalse(canAccept);

            manager.DeclineStoryResult(project, "12345", 3);
        }

        [Test]
        public void CanAcceptStory()
        {
            var project = new Project("SpecTests");
            var a1 = project.CreateArtifacts("local", "1.0.0");
            var a2 = project.CreateArtifacts("UAT", "1.0.1");
            var a3 = project.CreateArtifacts("local", "1.0.2");
            a1.RecordTestComplete();
            a2.RecordTestComplete();
            a3.RecordTestComplete();

            var manager = new StoryManager();
            var canAccept = manager.CanAcceptStory(project, "12345", 1);
            Assert.IsTrue(canAccept);
        }

        [Test]
        public void GetCurrentProjectStoryResultsTest()
        {
            var project = new Project("SpecTests");
            var a1 = project.CreateArtifacts("local", "1.0.0");
            var a2 = project.CreateArtifacts("UAT", "1.0.1");
            var a3 = project.CreateArtifacts("local", "1.0.2");
            a1.RecordTestComplete();
            a2.RecordTestComplete();
            a3.RecordTestComplete();

            var manager = new StoryManager();
            var results = manager.GetCurrentProjectStoryResults(project, "1111");
            var local = results.Environments[0];
            var uat = results.Environments[1];
            var localStory = local.Stories[0];
            var uatStory = uat.Stories[0];
            var testResult1 = localStory.Results[0];
            var testResult2 = uatStory.Results[0];

            Assert.AreEqual("local", local.Name);
            Assert.AreEqual("UAT", uat.Name);
            Assert.AreEqual("1111", localStory.Id);
            Assert.AreEqual("1111", uatStory.Id);
            Assert.AreEqual(3, testResult1.TestRun);
            Assert.AreEqual(2, testResult2.TestRun);
        }

        [Test]
        public void GetProjectStoryResultsTestByStoryTest()
        {
            var project = new Project("SpecTests");
            var a1 = project.CreateArtifacts("local", "1.0.0");
            var a2 = project.CreateArtifacts("UAT", "1.0.1");
            var a3 = project.CreateArtifacts("local", "1.0.2");
            a1.RecordTestComplete();
            a2.RecordTestComplete();
            a3.RecordTestComplete();

            var manager = new StoryManager();
            var results = manager.GetProjectStoryResults(project, "1111");
            var local = results.Environments[0];
            var uat = results.Environments[1];
            var localStory = local.Stories[0];
            var uatStory = uat.Stories[0];
            var testResult1 = localStory.Results[0];
            var testResult2 = uatStory.Results[0];
            var testResult3 = localStory.Results[1];

            Assert.AreEqual("local", local.Name);
            Assert.AreEqual("UAT", uat.Name);
            Assert.AreEqual("1111", localStory.Id);
            Assert.AreEqual("1111", uatStory.Id);
            Assert.AreEqual(1, testResult1.TestRun);
            Assert.AreEqual(2, testResult2.TestRun);
            Assert.AreEqual(3, testResult3.TestRun);
        }

        [Test]
        public void GetProjectStoryResultsTest()
        {
            var project = new Project("SpecTests");
            var a1 = project.CreateArtifacts("local", "1.0.0");
            var a2 = project.CreateArtifacts("UAT", "1.0.1");
            var a3 = project.CreateArtifacts("local", "1.0.2");
            a1.RecordTestComplete();
            a2.RecordTestComplete();
            a3.RecordTestComplete();

            var manager = new StoryManager();
            var results = manager.GetProjectStoryResults(project);
            
            var local = results.Environments[0];
            var uat = results.Environments[1];
            var localStory1 = local.Stories[0];
            var localStory2 = local.Stories[1];
            var uatStory1 = uat.Stories[0];
            var uatStory2 = uat.Stories[1];
            var testResult1a = localStory1.Results[0];
            var testResult1b = localStory2.Results[0];
            var testResult2a = uatStory1.Results[0];
            var testResult2b = uatStory2.Results[0];
            var testResult3a = localStory1.Results[1];
            var testResult3b = localStory2.Results[1];

            Assert.AreEqual("local", local.Name);
            Assert.AreEqual("UAT", uat.Name);
            Assert.AreEqual("12345", localStory1.Id);
            Assert.AreEqual("1111", localStory2.Id);
            Assert.AreEqual("12345", uatStory1.Id);
            Assert.AreEqual("1111", uatStory2.Id);
            Assert.AreEqual(1, testResult1a.TestRun); //local
            Assert.AreEqual(1, testResult1b.TestRun); //local
            Assert.AreEqual(2, testResult2a.TestRun); //uat
            Assert.AreEqual(2, testResult2b.TestRun); //uat
            Assert.AreEqual(3, testResult3a.TestRun); //local
            Assert.AreEqual(3, testResult3b.TestRun); //locla
        }
    }
}