using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using nFact.Shared;

namespace nFact.Engine
{
    public class TestResultsManager
    {
        public static StoryResult[] GetRallyStoryResults(XElement resultsXml)
        {
            var results = new List<StoryResult>();

            var fixtures = from f in resultsXml.XPathSelectElements("//test-suite[@type = 'TestFixture']")
                           select f;

            foreach (var fixture in fixtures)
            {
                var categories = from c in fixture.Descendants("category")
                                 let a = c.Attribute("name")
                                 where
                                     a != null &&
                                     a.Value.StartsWith("rally", StringComparison.InvariantCultureIgnoreCase)
                                 select c;

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
        public static StoryResult GetRallyStoryResult(string rallyId, XElement resultsXml)
        {
            var results = GetRallyStoryResults(resultsXml);
            return results.FirstOrDefault(r => r.Id.Equals(rallyId, StringComparison.InvariantCultureIgnoreCase));
        }

    }
}
