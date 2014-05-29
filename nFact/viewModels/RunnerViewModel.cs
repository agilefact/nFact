using System.Collections.Generic;

namespace nFact.viewModels
{
    public class RunnerViewModel : PageViewModel
    {
        private readonly SpecFlowScript _scriptModel;
        public string Script { get { return _scriptModel.Script; } }
        public List<string> Parameters { get; private set; }

        public RunnerViewModel(SpecFlowScript scriptModel, PageDataModel dataModel) : base(dataModel)
        {
            _scriptModel = scriptModel;
            ControlsVisible = false;
        }
    }
}
