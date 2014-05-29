using nFact.controllers;

namespace nFact.viewModels
{
    public class PageDataModelBuilder
    {
        public static PageDataModel Build(string spec)
        {
            var controller = new CommandController();
            var specs = controller.GetSpecs();
            var dataModel = controller.GetEngineDataModel();

            controller.SetSpec(spec);
            spec = controller.GetCurrentSpec();
            var environments = new string[0];
            string selectedEnvironment = null;
            if (spec != null)
            {
                var configController = new ConfigController(spec);
                environments = configController.GetEnvironments();
                selectedEnvironment = configController.GetSelectedEnvironment();
            }

            var pageModel = new PageDataModel();
            pageModel.specs = specs;
            pageModel.selectedSpec = spec;
            pageModel.video = dataModel.RecordVideo;
            pageModel.steps = dataModel.RecordSteps;
            pageModel.environments = environments;
            pageModel.selectedEnvironment = selectedEnvironment;

            return pageModel;
        }
    }
}