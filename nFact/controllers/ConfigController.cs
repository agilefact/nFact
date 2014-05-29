using System.Collections.Generic;
using System.Linq;
using nFact.Engine.Configuration;

namespace nFact.controllers
{
    public class ConfigController 
    {
        private readonly ProjectConfiguratonManager _manager;

        public string FilePath { get { return _manager.ConfigFilePath; } }
        public string FileName { get { return _manager.ConfigFileName; } }

        public ConfigController(string spec)
        {
            _manager = new ProjectConfiguratonManager(spec);
        }

        public string[] GetEnvironments()
        {
            return _manager.GetEnvironments().Select(e => e.Name).ToArray();
        }

        public string GetSelectedEnvironment()
        {
            return _manager.GetSelectedEnvironment();
        }

        public Dictionary<string, string> GetSettings(string environment)
        {
            var settings = _manager.Load(environment);
            if (settings == null)
                return new Dictionary<string, string>(0);

            return settings.ToDictionary(x => x.Name, x => x.Value);
        }

        public void Save(Dictionary<string, string> appSettings)
        {
            _manager.Save(appSettings);
        }
    }
}
