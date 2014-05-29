using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace AcceptanceTests.Scenarios
{
    public class ScanarioManager
    {
        public static int ScenarioCount { get; private set; }

        public static void Start()
        {
            ScenarioCount++;

            Console.Write("{Scenario.Start(");
            Console.Write(ScenarioCount);
            Console.WriteLine(")}");

            WriteLinks();
            WriteScenario();
        }

        public static void End()
        {
            Console.Write("{Scenario.End(");
            Console.Write(ScenarioCount);
            Console.WriteLine(")}");
        }

        private static void WriteScenario()
        {
            Console.WriteLine("Feature: " + FeatureContext.Current.FeatureInfo.Title);
            Console.WriteLine(FeatureContext.Current.FeatureInfo.Description);
            Console.WriteLine("\r\nScenario: " + ScenarioContext.Current.ScenarioInfo.Title);
        }

        private static void WriteLinks()
        {
            var tags = FeatureContext.Current.FeatureInfo.Tags;
            var linkTags = from t in tags
                           where string.Equals(t.Substring(0, 4), "link", StringComparison.CurrentCultureIgnoreCase)
                           select t;


            foreach (var link in linkTags)
            {
                Console.WriteLine(string.Format("@{0}", link));
            }
        }
        
    }
}
