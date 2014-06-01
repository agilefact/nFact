using System.Configuration;

namespace nFact.viewModels
{
    public class ControlsViewModel
    {
        private PageDataModel _dataModel;
        public bool SpecSelectorVisible
        {
            get
            {
                var specs = _dataModel.specs;
                return (specs != null && specs.Length > 0);
            }
        }

        public bool EnvironmentSelectorVisible
        {
            get
            {
                var env = _dataModel.environments;
                return (env != null && env.Length > 0);
            }
        }

        public bool HideControlsVisible { get; set; }
        public bool ControlsVisible { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public int PageMin { get; set; }
        public int PageMax { get; set; }
        public int PageCurrent { get; set; }
        public int NumOfPages { get; set; }
        public bool NextVisible { get; set; }
        public bool PrevVisible { get; set; }
        public string Spec { get { return _dataModel.selectedSpec; } }

        public ControlsViewModel(PageDataModel dataModel)
        {
            _dataModel = dataModel;
            ControlsVisible = true;
            var numOfPages = 5;
            var page = ConfigurationManager.AppSettings["ArtifactsPagination"];
            if (page != null)
            {
                int.TryParse(page, out numOfPages);
            }
            NumOfPages = numOfPages;
        }

        public void SetTestRunPagenation(int min, int current, int max)
        {
            Max = max;
            Min = min;

            if (current - NumOfPages < min)
            {
                PageMin = min;
                PageMax = min + NumOfPages - 1;
            }

            if ((current + NumOfPages) > max)
            {
                PageMin = max - NumOfPages + 1;
                PageMax = max;
            }

            if (min < PageMin)
                PrevVisible = true;

            if (max > PageMax)
                NextVisible = true;


            if (PageMax > max)
                PageMax = max;

            if (PageMin < min)
                PageMin = min;

            PageCurrent = current;
        }

        public void HideControls()
        {
            ControlsVisible = false;
            HideControlsVisible = true;
        }
    }
}
