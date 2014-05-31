using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;

namespace nFact.Engine.Test
{
    [TestFixture]
    public class TestResultsManagerTest
    {
        [Test]
        public void GetStoryResult()
        {
            var file = Path.Combine(Environment.CurrentDirectory, "TestResult.xml");
            var data = XElement.Load(file);
            var story = TestResultsManager.GetRallyStoryResult("12345", data);
            Assert.AreEqual("CustomerTransferSFundFeature", story.Name);
            Assert.AreEqual("Customer Transfer's Fund", story.Description);
            Assert.AreEqual(true, story.Executed);
            Assert.AreEqual(Result.Success, story.Result);
            Assert.IsTrue(story.Time.Equals(TimeSpan.FromSeconds(16)));
        }

        [Test]
        public void GetFailedStoryResults()
        {
            var file = Path.Combine(Environment.CurrentDirectory, "TestResult2.xml");
            var data = XElement.Load(file);
            var stories = TestResultsManager.GetRallyStoryResults(data);
            var story = stories[1];
            Assert.AreEqual(Result.Failure, story.Result);
            Assert.AreEqual(false, story.Success);
        }

        [Test]
        public void GetPendingResults()
        {
            var file = Path.Combine(Environment.CurrentDirectory, "TestResult3.xml");
            var data = XElement.Load(file);
            var stories = TestResultsManager.GetRallyStoryResults(data);
            var story = stories[1];
            Assert.AreEqual(Result.Pending, story.Result);
            Assert.AreEqual(false, story.Success);
        }
    }
}
