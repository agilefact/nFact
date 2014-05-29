using System;
using System.Collections.Generic;
using System.Linq;

namespace AcceptanceTests.Configuration
{
    public class TestConfigurationManagerBase
    {
        protected static Environment _environment;
        protected static System.Configuration.Configuration _config;

        public static TestSettingsCollection Settings { get { return GetAllTestSettings(); } }

        public IEnumerable<Setting> GetSettings()
        {
            var settings = GetAllTestSettings();
            return settings.Cast<Setting>();
        }

        public static Setting GetSetting(string name)
        {
            var settings = GetAllTestSettings();
            return settings.Cast<Setting>().FirstOrDefault(s => s.Name == name);
        }

        protected void LoadEnvironment()
        {
            var testConfiguration = GetTestConfiguration();
            var selected = testConfiguration.EnvironmentSettings.Selected;
            var environments = testConfiguration.EnvironmentSettings.Cast<Environment>();
            
            if (string.IsNullOrEmpty(selected) && environments.Any())
            {
                _environment = environments.First();
            }
            else
            {
                _environment = environments.FirstOrDefault(e => e.Name == selected);
                if (_environment == null)
                    throw new ApplicationException(string.Format("Could not select Environment Settings for the selected name '{0}'", selected));
            }
        }

        protected void LoadEnvironment(string environment)
        {
            if (environment == null)
                return;

            var testConfiguration = GetTestConfiguration();

            var environments = testConfiguration.EnvironmentSettings.Cast<Environment>();
            _environment = environments.FirstOrDefault(e => e.Name == environment);
            
        }

        protected static TestSettingsCollection GetAllTestSettings()
        {
            var allSettings = new TestSettingsCollection();
            var testSettings = GetTestConfiguration();
            foreach (Setting setting in testSettings.TestSettings)
            {   
                allSettings.Add(setting);
            }
            if (_environment != null)
            {
                foreach (Setting setting in _environment.TestSettings)
                {
                    if (allSettings.Cast<Setting>().Any(s => s.Name == setting.Name))
                        throw new ApplicationException(string.Format("There is a duplicate setting name: '{0}' in the configuration file", setting.Name));

                    allSettings.Add(setting);
                }
            }

            if (allSettings.Count == 0)
                throw new ApplicationException("No Test Settings have been loaded from the configuration file.");

            return allSettings;
        }

        protected static TestConfiguration GetTestConfiguration()
        {
            return (TestConfiguration)_config.Sections["TestConfiguration"];
        }
    }
}