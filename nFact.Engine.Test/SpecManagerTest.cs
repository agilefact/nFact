using NUnit.Framework;
using nFact.Engine.Model;

namespace nFact.Engine.Test
{
    [TestFixture]
    public class SpecManagerTest
    {
        [Test]
        public void LoadArtifacts()
        {
            var manager = new SpecManager();
            manager.LoadArtifacts("projects.xml");
            var project = manager.GetProject("SpecTests");
            Assert.AreEqual(6, project.TestRuns);
            var artifacts = project.Artifacts;
            Assert.AreEqual(1, artifacts[0].TestRun);
            Assert.AreEqual("20140601140432", artifacts[0].Date.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual("20140601150432", artifacts[1].Date.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual("20140602140432", artifacts[2].Date.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual("20140602150432", artifacts[3].Date.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual("20140603140432", artifacts[4].Date.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual("20140603150432", artifacts[5].Date.ToString("yyyyMMddHHmmss"));
        }
    }
}