using Nancy;

namespace nFact.modules
{
    public class ChartModule : NancyModule
    {
        public ChartModule()
        {
            Get["/{spec}/chart"] = p => View["story", null];
        }

    }
}
