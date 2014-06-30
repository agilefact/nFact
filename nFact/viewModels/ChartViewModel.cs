using System.Linq;
using Nancy.Json;
using nFact.controllers;

namespace nFact.viewModels
{
    public class ChartViewModel 
    {
        public ChartDataModel DataModel { get; private set; }

        public ChartViewModel(ChartDataModel dataModel)
        {
            DataModel = dataModel;
        }
    }

    public class ChartDataModel 
    {
        public string spec { get; set; }
        public string storyId { get; set; }
        public string BackUrl { get { return string.Format("/{0}/chart", spec); } }

        public string ToJSON()
        {
            var json = new JavaScriptSerializer().Serialize(this);
            return json;
        }
    }
}