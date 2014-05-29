using System.Collections.Generic;
using System.IO;
using System.Linq;
using AcceptanceTests.Configuration;

namespace nFact.Engine.Configuration
{
    public class ProjectConfiguratonManager
    {
        private readonly string _exeFilePath;
        private readonly string _exeFileName;
        private readonly TestConfigurationManagerAdmin _manager;
        private const string BinPath = @"bin\debug";

        public string ExeFilePath { get { return _exeFilePath; } }
        public string ExeFileName { get { return _exeFileName; } }
        public string ConfigFilePath { get; private set; }
        public string ConfigFileName { get; private set; }

        public ProjectConfiguratonManager(string spec)
        {
            _exeFilePath = Path.Combine(System.Environment.CurrentDirectory, "projects");
            _exeFilePath = Path.Combine(_exeFilePath, spec);
            _exeFilePath = Path.Combine(_exeFilePath, BinPath);
            ConfigFilePath = _exeFilePath;
            _exeFileName = string.Format("{0}.dll", spec);
            ConfigFileName = string.Format("{0}.config", _exeFileName);
            _exeFilePath = Path.Combine(_exeFilePath, _exeFileName);

            _manager = new TestConfigurationManagerAdmin(_exeFilePath);
        }

        public Environment[] GetEnvironments()
        {
            _manager.Load();
            return _manager.GetEnvironments();
        }

        public string GetSelectedEnvironment()
        {
            _manager.Load();
            return _manager.GetSelectedEnvironment();
        }

        public void SetEnvironment(string environment)
        {
            if (environment == null)
                return;
            
            var environments = _manager.GetEnvironments();
            if (environments.All(e => e.Name != environment)) 
                return;
            
            _manager.Load(environment);
            _manager.SetSelectedEnvironment(environment);
            _manager.Save();
        }

        public IEnumerable<Setting> Load(string environment)
        {
            _manager.Load(environment);
            return _manager.GetSettings();
        }

        public void Save(Dictionary<string, string> settings)
        {
            _manager.Load();
            foreach(var setting in settings)
            {
                _manager.SetSetting(setting.Key, setting.Value);
            }

            _manager.Save();
        }
    }
}