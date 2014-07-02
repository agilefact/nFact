using System.Configuration;
using nFact.viewModels.Controls;

namespace nFact.viewModels
{
    public class PageViewModel
    {
        public ControlsViewModel Controls { get; private set; }
        public PageDataModel DataModel { get; private set; }
        public bool ControlsVisible { get; set; }

        public PageViewModel(PageDataModel dataModel)
        {
            ControlsVisible = true;
            Controls = new ControlsViewModel(dataModel);
            DataModel = dataModel;
        }

        protected void SetTestRunPagination(int current, int max)
        {
            var artifactsLimit = 0;
            int.TryParse(ConfigurationManager.AppSettings["MaxArtifacts"], out artifactsLimit);

            if (artifactsLimit == 0)
                artifactsLimit = max;

            var min = max - artifactsLimit + 1;
            if (min < 1)
                min = 1;

            Controls.SetPagination(min, current, max);
        }
    }
}