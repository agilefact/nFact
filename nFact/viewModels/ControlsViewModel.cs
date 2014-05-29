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
        public int PageMax { get; set; }
        public int PageMin { get; set; }
        public int PageCurrent { get; set; }
        public int NumOfPages { get; set; }
        public bool PageMaxVisible { get; set; }
        public bool PageMinVisible { get; set; }
        public int MaxTest { get; set; }
        public string Spec { get { return _dataModel.selectedSpec; } }

        public ControlsViewModel(PageDataModel dataModel)
        {
            _dataModel = dataModel;
            ControlsVisible = true;
            NumOfPages = 5;
        }

        public void SetTestRunPagenation(int current, int max)
        {
            MaxTest = max;
            PageMinVisible = true;
            PageMaxVisible = true;

            if ((current - NumOfPages) <= 0)
            {
                PageMin = 1;
                PageMax = NumOfPages;
                PageMinVisible = false;
            }

            if ((current + NumOfPages) > max)
            {
                PageMin = max - NumOfPages + 1;
                PageMax = max;
                PageMaxVisible = false;
            }

            if (PageMax > max)
                PageMax = max;

            if (PageMin < 1)
                PageMin = 1;

            PageCurrent = current;
        }

        public void HideControls()
        {
            ControlsVisible = false;
            HideControlsVisible = true;
        }
    }
}
