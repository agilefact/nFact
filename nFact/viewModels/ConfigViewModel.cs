using System.Linq;
using nFact.controllers;

namespace nFact.viewModels
{
    public class ConfigViewModel : PageViewModel
    {
        private readonly ConfigController _controller;

        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public string Environment { get; private set; }
        public AppSetting[] AppSettings { get; private set; }

        public ConfigViewModel(PageDataModel pageModel) : base(pageModel)
        {
            Environment = pageModel.selectedEnvironment;
            ControlsVisible = false;
            _controller = new ConfigController(pageModel.selectedSpec);
            FileName = _controller.FileName;
            FilePath = _controller.FilePath;
            GetAppSettings();
        }

        private void GetAppSettings()
        {
            var settings = _controller.GetSettings(Environment);
            if (settings == null)
                return;

            AppSettings = settings.Select(a => new AppSetting(a.Key, a.Value)).ToArray();
        }
    }

    public struct AppSetting
    {
        public AppSetting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name;
        public string Value;
        public bool IsCheckBox {
            get
            {
                bool value;
                return (bool.TryParse(Value, out value));
            }
        }

        public string CheckBoxValue
        {
            get
            {
                bool value;
                if (bool.TryParse(Value, out value))
                {
                    if (value)
                        return "checked";
                }
                return string.Empty;
            }
        }
    }
}