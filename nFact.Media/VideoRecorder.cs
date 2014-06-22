using System;
using System.IO;
using Microsoft.Expression.Encoder.ScreenCapture;
using nFact.Shared;

namespace nFact.Media
{
    public class VideoRecorder : Recorder
    {
        private static ScreenCaptureJob scj;
        private IScriptLogger _logger = new ScriptLogger();
        private VideoEncoderMP4 _encoder = new VideoEncoderMP4();
        private string _filePathWmv;
        private string _filePathMp4;
        private volatile bool _isRunning;
        private string _filePathXesc;

        public override void Start(IScriptScenarioContext context)
        {
            base.Start(context);
      
            _logger.Log("Starting video recording...");
            var artifacts = context.Artifacts;

            var fileName = string.Format(@"Scen_{0}", context.ScenarioCount);
            _filePathXesc = Path.Combine(artifacts.FilePath, string.Format(@"{0}.xesc", fileName));
            _filePathWmv = Path.Combine(artifacts.FilePath, string.Format(@"{0}.wmv", fileName));
            _filePathMp4 = Path.Combine(artifacts.FilePath, string.Format(@"{0}.mp4", fileName));

            Delete(_filePathXesc);
            Delete(_filePathWmv);
            Delete(_filePathMp4);

            scj = new ScreenCaptureJob();
            scj.OutputScreenCaptureFileName = _filePathXesc;
            scj.Duration = TimeSpan.FromMinutes(MaxRecordingTime);

            scj.Start();
            _isRunning = true;

            _logger.Log("Video recording started");
        }

        private void Delete(string file)
        {
            if (File.Exists(file))
                File.Delete(file);
        }

        public override void End(IScriptScenarioContext context)
        {
            base.End(context);

            if (!_isRunning)
                return;

            if (scj != null)
            {
                _logger.Log("Stopping video recording...");
                scj.Stop();
                scj = null;
                _logger.Log("Video recording stopped");

                _isRunning = false;
                _encoder.Encode(_filePathXesc, _filePathMp4);
                //_encoder.Encode(_filePathXesc, artifacts.FilePath);
            }
        }

        public override void Dispose()
        {
            if (scj == null) return;
            scj.Stop();
            scj = null;
        }
    }   
}