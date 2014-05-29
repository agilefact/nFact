using System;
using System.Configuration;
using System.Linq;
using AcceptanceTests.Configuration;
using Environment = AcceptanceTests.Configuration.Environment;

namespace nFact.Engine.Configuration
{
    public class TestConfigurationManagerAdmin : TestConfigurationManagerBase
    {
        public TestConfigurationManagerAdmin(string filePath)
        {
            _config = ConfigurationManager.OpenExeConfiguration(filePath);
        }

        public Environment[] GetEnvironments()
        {
            if (_config == null)
                throw new ApplicationException("Test Configuration has not been loaded. Call TestConfigurationManager.Load() first.");

            var testConfig = GetTestConfiguration();
            return testConfig.EnvironmentSettings.Cast<Environment>().ToArray();
        }

        public void Load()
        {
            LoadEnvironment();
        }

        public void Load(string environment)
        {
            LoadEnvironment(environment);
        }

        public string GetSelectedEnvironment()
        {
            var testConfiguration = GetTestConfiguration();
            return testConfiguration.EnvironmentSettings.Selected;
        }

        public void SetSelectedEnvironment(string environment)
        {
            var testConfiguration = GetTestConfiguration();
            testConfiguration.EnvironmentSettings.Selected = environment;
        }

        public void SetSetting(string name, string value)
        {
            var testSettings = GetTestConfiguration();
            var settings = testSettings.TestSettings.Cast<Setting>();
            var setting = settings.FirstOrDefault(s => s.Name == name);
            if (setting == null && _environment != null)
            {
                settings = _environment.TestSettings.Cast<Setting>();
                setting = settings.FirstOrDefault(s => s.Name == name); if (setting == null)
                    throw new ApplicationException(string.Format("Could not find setting '{0}' for environment '{1}' in configuration file.", name, _environment.Name));
            }

            if (setting == null)
                throw new ApplicationException(string.Format("Could not find setting '{0}' in configuration file.", name));

            if (setting.Editable)
                setting.Value = value;
        }

        public void Save()
        {
            _config.Save();
        }
    }
}