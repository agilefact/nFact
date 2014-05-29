using System;
using System.IO;
using ScriptRunner;
using nFact.Engine.Model;
using nFact.Shared;

namespace nFact.Engine.Commands
{
    public class CommandHandler : ICommandHandler, IDisposable
    {
        private readonly IScriptScenarioContext _context;
        private readonly ScriptEngineModel _model;
        private readonly IRecorder _videoRecorder;
        private readonly IRecorder _slidesRecorder;

        public CommandHandler(CommandContext context)
        {
            _context = context.ScriptContext;
            _model = context.Model;
            _videoRecorder = context.VideoRecorder;
            _slidesRecorder = context.SlidesRecorder;
        }

        public void Parse(string messageItem)
        {
            using (var ms = new MemoryStream())
            {
                var sw = new StreamWriter(ms);
                sw.WriteLine(messageItem);
                sw.Flush();
                ms.Position = 0;
                var sr = new StreamReader(ms);
                var text = sr.ReadLine();
                while (text != null)
                {
                    string command, parameter;

                    CommandParser.GetCommand(text, out command, out parameter);
                    ProcessStatement(command, parameter);

                    text = sr.ReadLine();
                }
            }
        }


        private void ProcessStatement(string command, string parameter)
        {
            if (command == null)
                return;

            try
            {
                if (command.StartsWith(CommandParser.ScenarioStart))
                {
                    ScenarioStartCommand(parameter);
                }

                if (command.StartsWith(CommandParser.ScenarioEnd))
                {
                    ScenarioEndCommand();
                }
            }
            catch(Exception)
            {
                _videoRecorder.End(_context);
                _slidesRecorder.End(_context);
                throw;
            }
        }

        private void ScenarioStartCommand(string parameter)
        {
            int scenarioCount;
            int.TryParse(parameter, out scenarioCount);
            if (scenarioCount == 0)
                return;

            _context.ScenarioCount = scenarioCount;

            if (_model.RecordVideo)
                _videoRecorder.Start(_context);

            if (_model.RecordSteps)
            {
                _slidesRecorder.Start(_context);
            }
        }

        private void ScenarioEndCommand()
        {
            if (_model.RecordVideo)
                _videoRecorder.End(_context);

            if (_model.RecordSteps)
            {
                _slidesRecorder.End(_context);
            }
        }

        public void Dispose()
        {
            _videoRecorder.Dispose();
            _slidesRecorder.Dispose();
        }
    }
}