using Nancy;
using nFact.viewModels;

namespace nFact.modules
{
    public class ChartModule : NancyModule
    {
        public ChartModule()
        {
            Get["/{spec}/chart"] = p => View["charts/story", BuildViewModel(p.spec)];
        }

        private ChartViewModel BuildViewModel(string spec)
        {
            var pageModel = PageDataModelBuilder.Build(spec);

            return new ChartViewModel(pageModel);
        }
    }
}
