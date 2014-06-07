using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcceptanceTests.Configuration;
using TechTalk.SpecFlow;

namespace AcceptanceTests.TestData
{
    public class Scenario
    {
        public static void SetupData(string script)
        {
            try
            {
                TestConfigurationManager.Load();

                var dbFit = new DbFitHandler();
                dbFit.Setup(script);
            }
            catch
            {
            }
        }

        public static void TeardownData(string script)
        {
            TeardownData(script, null);
        }

        public static void TeardownData(string script, string tag)
        {
            try
            {
                TestConfigurationManager.Load();

                var scenarioInfo = ScenarioContext.Current.ScenarioInfo;
                if (tag == null || scenarioInfo.Tags.Any(t => t == tag))
                {
                    var dbFit = new DbFitHandler();
                    dbFit.Teardown(script);
                }
            }
            catch
            {
            }
        }
    }
}
