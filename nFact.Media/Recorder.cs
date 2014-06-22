using System.Configuration;
using System.Timers;
using nFact.Shared;

namespace nFact.Media
{
    public abstract class Recorder : IRecorder
    {
        private IScriptLogger _logger = new ScriptLogger();

        private static Timer _timer;
        protected int MaxRecordingTime;
        private const int DefaultRecTime = 8;

        protected Recorder()
        {
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        }

        public virtual void Start(IScriptScenarioContext context)
        {
            var txtRecordingTime = ConfigurationManager.AppSettings["MaxRecorderTimeMins"];
            if (!int.TryParse(txtRecordingTime, out MaxRecordingTime))
                MaxRecordingTime = DefaultRecTime;

            _timer.Interval = MaxRecordingTime * 1000;
            _timer.Enabled = true;   
        }

        public virtual void End(IScriptScenarioContext context)
        {
            _timer.Enabled = false;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _logger.Log(string.Format("Max Recording Time of {0} mins reached", MaxRecordingTime));

            Dispose();
        }

        public abstract void Dispose();
    }
}