using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcceptanceTests.Configuration;
using AcceptanceTests.TestData;
using TechTalk.SpecFlow;

namespace AcceptanceTests.Scenarios
{
    public class ScanarioManager
    {
        public static int ScenarioCount { get; private set; }
        private static bool _scenarioRunning;

        public static void Start()
        {
            if(_scenarioRunning)
                End();

            Scenario.Start();

            _scenarioRunning = true;
            ScenarioCount++;

            Console.Write("{Scenario.Start(");
            Console.Write(ScenarioCount);
            Console.WriteLine(")}");

            WriteScenario();
            WriteLinks("link");
            WriteLinks("jira");
            WriteLinks("rally");
        }

        public static void End()
        {
            Console.Write("{Scenario.End(");
            Console.Write(ScenarioCount);
            Console.WriteLine(")}");
        }

        private static void WriteScenario()
        {
            Console.WriteLine("Story: " + FeatureContext.Current.FeatureInfo.Title);
            Console.WriteLine(FeatureContext.Current.FeatureInfo.Description);
            Console.WriteLine("\r\nScenario: " + ScenarioContext.Current.ScenarioInfo.Title);
        }

        private static void WriteLinks(string tag)
        {
            var tags = FeatureContext.Current.FeatureInfo.Tags;
            var selectedTags = from t in tags
                           where string.Equals(t.Substring(0, tag.Length), tag, StringComparison.CurrentCultureIgnoreCase)
                           select t;


            foreach (var t in selectedTags)
            {
                switch(tag.ToLower())
                {
                    case "jira":
                        WriteJiraLink("jira", t);
                        break;
                }

                Console.WriteLine(string.Format("@{0}", t));
            }
        }

        private static void WriteJiraLink(string label, string tag)
        {
            var labelLength = label.Length;
            var domain = TestConfigurationManager.Settings["JIRA.Domain"];
            var project = TestConfigurationManager.Settings["JIRA.Project"];
            var start = labelLength + 1;
            var issue = tag.Substring(start, tag.Length - start + 1);
            var url = string.Format("{0}/browse/{1}-{2}", domain, project, issue);
            Console.Write("@link{");
            Console.Write(url);
            Console.Write("}");
        }
    }
}
