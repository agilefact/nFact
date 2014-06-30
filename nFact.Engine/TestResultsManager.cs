using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using nFact.Engine.Model;
using nFact.Shared;

namespace nFact.Engine
{
    public class TestResultsManager
    {
        public static StoryResult[] GetStoryResults(string file)
        {
            var resultsXml = XElement.Load(file);
            var results = new List<StoryResult>();

            var fixtures = from f in resultsXml.XPathSelectElements("//test-suite[@type = 'TestFixture']")
                           select f;

            foreach (var fixture in fixtures)
            {
                var categories = GetStoryCategories(fixture);

                foreach (var category in categories)
                {
                    string id;
                    CommandParser.GetContents(category.Attribute("name").Value, '{', '}', out id);

                    var result = StoryResult.Parse(fixture, id);
                    results.Add(result);
                }
            }

            return results.ToArray();
        }

        private static IEnumerable<XElement> GetStoryCategories(XElement fixture)
        {
            var categories = from c in fixture.Descendants("category")
                             let a = c.Attribute("name")
                             where
                                 a != null &&
                                 (
                                     a.Value.StartsWith("rally", StringComparison.InvariantCultureIgnoreCase) ||
                                     a.Value.StartsWith("jira", StringComparison.InvariantCultureIgnoreCase)
                                 )
                             select c;
            return categories;
        }

        public static void AcceptStoryResult(string storyId, string file)
        {
            var resultsXml = XElement.Load(file);

            var storyResult = FindFixture(resultsXml, storyId);
            if (storyResult == null)
                throw new ApplicationException(string.Format("Unable to find story Id: {0}", storyId));

            var xAttributes = storyResult.Attributes("accepted");
            if (xAttributes.Any())
                return;

            storyResult.Add(new XAttribute("accepted", true));

            resultsXml.Save(file);
        }

        public static void DeclineStoryResult(string storyId, string file)
        {
            var resultsXml = XElement.Load(file);

            var storyResult = FindFixture(resultsXml, storyId); if (storyResult == null)
                throw new ApplicationException(string.Format("Unable to find story Id: {0}", storyId));

            var xAttribute = storyResult.Attribute("accepted");
            if (xAttribute == null)
                return;

            xAttribute.Remove();

            resultsXml.Save(file);
        }

        internal static XElement FindFixture(XElement results, string storyId)
        {
            XElement storyElement;
            return FindFixture(results, storyId, out storyElement);
        }

        internal static XElement FindFixture(XElement results, string storyId, out XElement storyElement)
        {
            var fixtures = from f in results.XPathSelectElements("//test-suite[@type = 'TestFixture']")
                           select f;

            foreach (var fixture in fixtures)
            {
                var categories = GetStoryCategories(fixture);

                foreach (var category in categories)
                {
                    string id;
                    CommandParser.GetContents(category.Attribute("name").Value, '{', '}', out id);

                    if (id.Equals(storyId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        storyElement = category;
                        return fixture;
                    }
                }
            }

            storyElement = null;
            return null;
        }

        public static StoryResult GetRallyStoryResult(string rallyId, string file)
        {
            var results = GetStoryResults(file);
            return results.FirstOrDefault(r => r.Id.Equals(rallyId, StringComparison.InvariantCultureIgnoreCase));
        }

    }
}
