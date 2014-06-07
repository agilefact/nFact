using System;
using System.Linq;
using AcceptanceTests.Configuration;
using AcceptanceTests.TestData;
using TechTalk.SpecFlow;

namespace DBFitTestData.Steps
{
    
    [Binding]
    public class TestDataSetupSteps
    {
        [BeforeScenario("dbFit")]
        public void BeforeScenario()
        {
            Scenario.SetupData("CommBiz.AssetFinance");
        }

        [AfterScenario("dbFit")]
        public void AfterScenario()
        {
            Scenario.TeardownData("CommBiz.AssetFinanceTearDown");
        }

        [Given(@"Any Scenarrio")]
        public void GivenAnyScenarrio()
        {
        }
        
        [Then(@"Test data should be inserted then rolled back")]
        public void ThenTestDataShouldBeInsertedThenRolledBack()
        {
            throw new ApplicationException();
        }
    }
}
