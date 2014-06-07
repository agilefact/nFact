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
            try
            {
                TestConfigurationManager.Load();

                var dbFit = new DbFitHandler();
                dbFit.Teardown(script);
            }
            catch
            {
            }
        }
    }
}
