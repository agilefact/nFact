using System;
using System.Configuration;
using AcceptanceTests.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using TechTalk.SpecFlow;

namespace SpecTests
{
    public class Environment
    {
        private static IWebDriver driver;

        public static IWebDriver Driver
        {
            get { return driver ?? (driver = GetWebDriver()); }
        }

        private static IWebDriver GetWebDriver()
        {
            TestConfigurationManager.Load();
            switch (TestConfigurationManager.Settings["Browser"])
            {
                case "Chrome":
                    return new ChromeDriver();
                default:
                    return new FirefoxDriver();
            }
        }

        public static void Navigate(string url)
        {

            var urlbase = TestConfigurationManager.Settings["UrlBase"];
            url = string.Format("{0}/{1}", urlbase, url);

            // Attempt to navigate to requested page
            Driver.Navigate().GoToUrl(url);

        }

        public static void CloseWebDriver()
        {
            if (driver == null) return;
            Driver.Close();
            Driver.Quit();
            driver = null;
        }
    }
}