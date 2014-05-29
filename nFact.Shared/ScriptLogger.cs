using System;
using log4net;

namespace nFact.Shared
{
    public class ScriptLogger : IScriptLogger
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ScriptLogger));

        public void Log(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var handler = MessageHandler.Instance;
            _log.Info(text);
            Console.WriteLine(text);
            handler.AddMessage(string.Format("{0}{1}", text, Environment.NewLine), MessageState.OK);
        }

        public void LogProgress(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var handler = MessageHandler.Instance;
            Console.Write(text);
            handler.AddMessage(text, MessageState.Progress);
        }

        public void LogError(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var handler = MessageHandler.Instance;
            handler.AddMessage(text, MessageState.Error);
            _log.Error(text);
            Console.WriteLine(text);
        }

        public static void Close()
        {
            _log.Logger.Repository.Shutdown();
        }
    }
}