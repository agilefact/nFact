using System.Configuration;

namespace AcceptanceTests.Configuration
{
    public class Environment : ConfigurationSection
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("TestSettings")]
        [ConfigurationCollection(typeof(EnvironmentSettings), AddItemName = "Setting")]
        public TestSettingsCollection TestSettings { get { return (TestSettingsCollection)base["TestSettings"]; } }
    }
}