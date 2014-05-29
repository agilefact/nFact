
using System;
using System.Threading;
using AcceptanceTests.ScreenCapture;
using NUnit.Framework;
using SpecTests.Pages;
using TechTalk.SpecFlow;

namespace SpecTests.Steps
{
    [Binding]
    public class CustomerTransferSFundSteps
    {
        FundTransferPage _ftPage = new FundTransferPage(Environment.Driver);
        ScreenshotCapture _screenshot = new ScreenshotCapture(Environment.Driver);

        [Given(@"the user is on Fund Transfer Page")]
        public void GivenTheUserIsOnFundTransferPage()
        {
            Environment.Navigate("fundTransfer.html");
        }
        
        [When(@"he enters ""(.*)"" as payee name")]
        public void WhenHeEntersAsPayeeName(string payeeName)
        {
            _ftPage.payeeNameField.SendKeys(payeeName);
            _screenshot.Take();
        }
        [When(@"he enters ""(.*)"" as amount")]
        public void WhenUserEneteredIntoTheAmountField(string amount)
        {
            _ftPage.amountField.SendKeys(amount);
            _screenshot.Take();
        }

        [When(@"he enters ""(.*)"" as amount above his limit")]
        public void WhenUserEneteredIntoTheAmountFieldAboveLimit(string amount)
        {
            _ftPage.amountField.SendKeys(amount);
            _screenshot.Take();
        }

        [When(@"he Submits request for Fund Transfer")]
        public void WhenUserPressTransferButton()
        {
            Thread.Sleep(5000);
            _ftPage.transferButton.Click();
            _screenshot.Take();
        }

        [Then(@"ensure the fund transfer is complete with ""(.*)"" message")]
        public void ThenFundTransferIsComplete(string message)
        {
            Assert.AreEqual(message, _ftPage.messageLabel.Text);
            _screenshot.Take();
        }

        [Then(@"ensure a transaction failure message ""(.*)"" is displayed")]
        public void ThenFundTransferIsFailed(string message)
        {
            Assert.AreEqual(message, _ftPage.messageLabel.Text);
            _screenshot.Take();
        }
    }
}
