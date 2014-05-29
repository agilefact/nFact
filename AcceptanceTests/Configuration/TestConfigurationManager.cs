using System.Configuration;

namespace AcceptanceTests.Configuration
{
    public class TestConfigurationManager : TestConfigurationManagerBase
    {
        public TestConfigurationManager()
        {
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public static void Load()
        {
            var manager = new TestConfigurationManager();
            manager.LoadEnvironment();
        }
    }
}