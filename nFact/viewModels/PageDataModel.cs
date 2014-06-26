using Nancy.Json;

namespace nFact.viewModels
{
    public class PageDataModel
    {
        public string[] specs { get; set; }
        public string selectedSpec { get; set; }
        public bool video { get; set; }
        public bool steps { get; set; }
        public string[] environments { get; set; }
        public string selectedEnvironment { get; set; }
        public int testRun { get; set; }

        public string ToJSON()
        {
            var json = new JavaScriptSerializer().Serialize(this);
            return json;
        }
    }
}