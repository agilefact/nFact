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

        protected void SetTestRunPagenation(int current, int max)
        {
            Controls.SetTestRunPagenation(current, max);
        }
    }
}