using System;
using System.IO;
using nFact.Shared;

namespace nFact.Media
{
    public class SlidesRecorder : Recorder
    {
        private const string PSRExe = "psr.exe";

        private readonly IScriptLogger _logger = new ScriptLogger();
        private volatile bool _isRunning;

        public override void Start(IScriptScenarioContext context)
        {
            base.Start(context);

            _logger.Log("Starting slides recording...");
            var artifacts = context.Artifacts;
            var fileName = Path.Combine(artifacts.FilePath, string.Format(@"steps_{0}.zip", context.ScenarioCount));
            if (File.Exists(fileName))
                File.Delete(fileName);

            // Kill all running PSR processes
            ProcessController.TryKillProcess(PSRExe);

            // Start PSR
            ProcessController.InvokeProcess(PSRExe, String.Format("/start /output \"{0}\" /gui 0 /sc 1 /sketch 1 /maxsc 100", fileName));

            _isRunning = true;
            _logger.Log(string.Format("Steps recording started. File: {0}", fileName));
        }

        public override void End(IScriptScenarioContext context)
        {
            base.End(context);

            if (!_isRunning)
                return;

            try
            {
                _logger.Log("Stopping slides recording...");
                ProcessController.InvokeProcess(PSRExe, @"/stop").WaitForExit(60000);
                _logger.Log("Steps recording stopped.");
                _isRunning = false;
            }
            catch (Exception)
            {
            }

        }

        public override void Dispose()
        {
            ProcessController.TryKillProcess(PSRExe);
        }
        
    }
}