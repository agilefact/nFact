using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using nFact.Engine.Model;

namespace nFact.Engine.Test
{
    [TestFixture]
    public class TestResultsManagerTest
    {
        [Test]
        public void GetStoryResult()
        {
            var file = Path.Combine(Environment.CurrentDirectory, "TestResult.xml");
            var story = TestResultsManager.GetRallyStoryResult("12345", file);
            Assert.AreEqual("CustomerTransferSFundFeature", story.Name);
            Assert.AreEqual("Customer Transfer's Fund", story.Description);
            Assert.AreEqual(true, story.Executed);
            Assert.AreEqual(Result.Success, story.Result);
            Assert.AreEqual(16, story.Seconds);
        }

        [Test]
        public void GetFailedStoryResults()
        {
            var file = Path.Combine(Environment.CurrentDirectory, "TestResult2.xml");
            var stories = TestResultsManager.GetRallyStoryResults(file);
            var story = stories[1];
            Assert.AreEqual(Result.Failure, story.Result);
            Assert.AreEqual(false, story.Success);
        }

        [Test]
        public void GetPendingResults()
        {
            var file = Path.Combine(Environment.CurrentDirectory, "TestResult3.xml");
            var stories = TestResultsManager.GetRallyStoryResults(file);
            var story = stories[1];
            Assert.AreEqual(Result.Pending, story.Result);
            Assert.AreEqual(false, story.Success);
        }
    }
}
