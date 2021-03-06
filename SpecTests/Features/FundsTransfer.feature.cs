﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.2.1
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.1022
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SpecTests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.2.1")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Customer Transfer\'s Fund")]
    public partial class CustomerTransferSFundFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "FundsTransfer.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Customer Transfer\'s Fund", "     As a customer\r\n     I want to transfer funds\r\n     so that I can send money " +
                    "to my friends and family", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Valid Payee")]
        public virtual void ValidPayee()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Valid Payee", ((string[])(null)));
#line 6
this.ScenarioSetup(scenarioInfo);
#line 7
       testRunner.Given("the user is on Fund Transfer Page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
       testRunner.When("he enters \"Jim\" as payee name", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 9
       testRunner.And("he enters \"100\" as amount", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 10
       testRunner.And("he Submits request for Fund Transfer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 11
       testRunner.Then("ensure the fund transfer is complete with \"$100 transferred successfully to Jim!!" +
                    "\" message", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
