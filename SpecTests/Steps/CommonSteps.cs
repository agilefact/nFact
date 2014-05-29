using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcceptanceTests.Scenarios;
using TechTalk.SpecFlow;

namespace SpecTests.Steps
{
    [Binding]
    public class CommonSteps
    {
        [BeforeScenario]
        public void BeforeScenario()
        {
            ScanarioManager.Start();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            ScanarioManager.End();
            Environment.CloseWebDriver();
        }
    }
}
