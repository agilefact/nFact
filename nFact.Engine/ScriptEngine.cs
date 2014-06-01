using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using ScriptRunner;
using nFact.Engine.Commands;
using nFact.Engine.Configuration;
using nFact.Engine.Model;
using nFact.Shared;

namespace nFact.Engine
{
    public class ScriptEngine
    {
        private SpecManager _specManager;
        private EngineManager _scriptEngine;
        private static ScriptEngine _instance;
        private ProjectArtifacts _artifacts;
        private ScriptEngineModel _model;
        private CommandHandler _commandHandler;

        private IScriptLogger _logger = new ScriptLogger();
        private ScriptMessageState _scriptState;
        private volatile bool _scriptRunning;

        public SpecManager SpecManager { get { return _specManager; } }

        public void Init()
        {
            _scriptEngine = new EngineManager();
            _scriptEngine.ScriptComplete += OnScriptComplete;
            _scriptEngine.NewMessageReady += OnNewMessageReady;
            _scriptEngine.ScriptStopped += OnScriptComplete;

            _specManager = new SpecManager();
            _specManager.Init();

            _model = new ScriptEngineModel();
            _model.Specs = _specManager.GetProjectSpecs();

        }

        void OnNewMessageReady(object sender, EventArgs e)
        {
            var message = _scriptEngine.GetMessage();
            if (_scriptState != ScriptMessageState.Error)
                _scriptState = message.State;

            if (message.State == ScriptMessageState.Error)
                _logger.LogError(message.Message);
            else
                _logger.Log(message.Message);
        }

        void OnScriptComplete(object sender, EventArgs e)
        {
            ScriptLogger.Close();

            _artifacts.CheckFiles();
            _artifacts.RecordTestComplete();
            _specManager.SaveArtifacts();

            _scriptRunning = false;
        }

        public static ScriptEngine Instance
        {
            get { return _instance ?? (_instance = new ScriptEngine()); }
        }

        public void Start(IScript model, string environment)
        {
            if (_scriptRunning)
                throw new ApplicationException("Cannot execute more than one script at a time.");

            _scriptRunning = true;
            _scriptState = ScriptMessageState.OK;

            var messageHandle = MessageHandler.Instance;
            messageHandle.Reset();

            CreateArtifacts(model);

            var context = new ScriptContext(model, _artifacts);
            var handlerContext = new CommandContext(_model, context);
            _commandHandler = new CommandHandler(handlerContext);

            log4net.Config.XmlConfigurator.Configure();
            LogConfigurationSettings(model.Spec,environment);
            LogScriptParameters(context.Parameters);

            _scriptEngine.AddCommandHandler(_commandHandler);
            _scriptEngine.Start(context);
        }

        private void LogScriptParameters(Hashtable parameters)
        {
            var builder = new StringBuilder();
            builder.AppendLine("**** Script Parameters ****");
            foreach (DictionaryEntry parameter in parameters)
            {
                builder.AppendLine(string.Format("{0}: {1}", parameter.Key, parameter.Value));
            }
            builder.Append("***************************");
            _logger.Log(builder.ToString());
        }

        private void LogConfigurationSettings(string spec, string environment)
        {
            var configManager = new ProjectConfiguratonManager(spec);
            var settings = configManager.Load(environment);

            if (settings == null)
                return;

            var builder = new StringBuilder();
            builder.AppendLine("**** Environment Configuration ****");
            foreach (var setting in settings)
            {
                if (!setting.Visible) continue;

                var config = string.Format("{0}: {1}", setting.Name, setting.Value);
                builder.AppendLine(config);
            }
            builder.Append("***********************************");
            _logger.Log(builder.ToString());
        }

        public void Stop()
        {
            if (_commandHandler != null)
                _commandHandler.Dispose();

            _scriptEngine.Stop();
        }

        private void CreateArtifacts(IScript model)
        {
            _artifacts = _specManager.CreateArtifacts(model);

            int maxVersions = 0;
            int.TryParse(ConfigurationManager.AppSettings["MaxArtifacts"], out maxVersions);
            _artifacts.DeleteExpiredArtifacts(maxVersions);
        }

        public ScriptMessage GetMessage()
        {
            var message = new ScriptMessage();

            var handler = MessageHandler.Instance;

            message.Message = handler.GetMessages();
            message.State = _scriptState;

            if (handler.State == MessageState.Error)
                message.State = ScriptMessageState.Error;

            return message;
        }

        public void SetModel(ScriptEngineModel scriptEngineModel)
        {
            _model = scriptEngineModel;
        }

        public ScriptEngineModel GetModel()
        {
            return _model;
        }

        public ProjectArtifacts GetArtifacts()
        {
            return _artifacts;
        }
    }
}