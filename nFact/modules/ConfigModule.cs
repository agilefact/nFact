using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using nFact.controllers;
using nFact.viewModels;

namespace nFact.modules
{
    public class ConfigModule : NancyModule
    {
        public ConfigModule()
        {
            Get["/{spec}/config"] = p => View["config", BuildViewModel(p.spec)];
            Post["/{spec}/config"] = p => SaveConfig(p);
        }

        private ConfigViewModel BuildViewModel(string spec)
        {
            var pageModel = PageDataModelBuilder.Build(spec);
            return new ConfigViewModel(pageModel);
        }

        private dynamic SaveConfig(dynamic p)
        {
            Configuration model = this.Bind();

            var controller = new ConfigController(p.spec);
            var appSettings = model.appSettings.ToDictionary(c => c.name, c => c.value);
            controller.Save(appSettings);

            return HttpStatusCode.OK;
        }
    }

    
    public class Configuration
    {
        public string spec { get; set; }

        public IEnumerable<AppSetting> appSettings { get; set; }
    }

    public class AppSetting
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}
