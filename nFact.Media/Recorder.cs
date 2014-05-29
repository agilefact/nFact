using System.Configuration;
using System.Timers;
using nFact.Shared;

namespace nFact.Media
{
    public abstract class Recorder : IRecorder
    {
        private IScriptLogger _logger = new ScriptLogger();

        private static Timer _timer;
        private int _maxRecordingTime;
        private const int DefaultRecTime = 8;

        protected Recorder()
        {
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        }

        public virtual void Start(IScriptScenarioContext context)
        {
            var txtRecordingTime = ConfigurationManager.AppSettings["MaxRecorderTimeMins"];
            if (!int.TryParse(txtRecordingTime, out _maxRecordingTime))
                _maxRecordingTime = DefaultRecTime;

            _timer.Interval = _maxRecordingTime * 1000;
            _timer.Enabled = true;   
        }

        public virtual void End(IScriptScenarioContext context)
        {
            _timer.Enabled = false;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _logger.Log(string.Format("Max Recording Time of {0} mins reached", _maxRecordingTime));

            End(null);
        }

        public abstract void Dispose();
    }
}