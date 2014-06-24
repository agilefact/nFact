using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcceptanceTests.Configuration;
using AcceptanceTests.Scenarios;
using TechTalk.SpecFlow;

namespace AcceptanceTests.TestData
{
    public class Scenario
    {
        public static bool SetupExceptionOccurred;
        public static Exception LastException;

        public static void SetupData(string script)
        {
            SetupExceptionOccurred = false;
            try
            {
                TestConfigurationManager.Load();

                var dbFit = new DbFitHandler();
                dbFit.Setup(script);
            }
            catch (Exception e)
            {
                SetupExceptionOccurred = true;
                LastException = e;
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
            catch (Exception e)
            {
                SetupExceptionOccurred = true;
                LastException = e;
            }
        }

        internal static void Start()
        {
            if(SetupExceptionOccurred)
            {
                SetupExceptionOccurred = false;
                throw new ApplicationException("Exception occurred during Scenario Setup", LastException);
            }

            SetupExceptionOccurred = false;
        }
    }
}
