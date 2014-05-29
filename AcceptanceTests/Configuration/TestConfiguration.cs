using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace AcceptanceTests.Configuration
{
    public class TestConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("TestSettings")]
        [ConfigurationCollection(typeof(EnvironmentSettings), AddItemName = "Setting")]
        public TestSettingsCollection TestSettings { get { return (TestSettingsCollection)base["TestSettings"]; } }

        [ConfigurationProperty("EnvironmentSettings")]
        [ConfigurationCollection(typeof(EnvironmentSettings), AddItemName = "Environment")]
        public EnvironmentSettings EnvironmentSettings { get { return (EnvironmentSettings)base["EnvironmentSettings"]; } }
    }
}
