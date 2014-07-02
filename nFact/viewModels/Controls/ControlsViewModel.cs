using System.Configuration;

namespace nFact.viewModels.Controls
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
        public string Spec { get { return _dataModel.selectedSpec; } }
        public PaginationViewModel Pages { get; set; }

        public ControlsViewModel(PageDataModel dataModel)
        {
            _dataModel = dataModel;
            ControlsVisible = true;
            var numOfPages = 5;
            var page = ConfigurationManager.AppSettings["ReportPagination"];
            if (page != null)
            {
                int.TryParse(page, out numOfPages);
            }

            Pages = new PaginationViewModel(dataModel.selectedSpec, numOfPages);
        }

        public void HideControls()
        {
            ControlsVisible = false;
            HideControlsVisible = true;
        }

        public void SetPagination(int min, int current, int max)
        {
            Pages.Set(min, current, max);
        }
    }
}
