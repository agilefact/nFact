using System;
using Nancy;
using nFact.Engine;
using nFact.controllers;
using nFact.viewModels;

namespace nFact.modules
{
    public class RunnerModule : NancyModule 
    {
        RunnerController _controller = new RunnerController();
        public RunnerModule()
        {
            Get["/{spec}/run"] = p => Run(p.spec);
            Get["/{spec}/output"] = p => SendMessage();
            Post["/{spec}/stop"] = p => Stop();
            Post["/{spec}/restart"] = p => Restart();
        }

        private dynamic Stop()
        {
            ScriptEngine.Instance.Stop();
            return HttpStatusCode.OK;
        }

        private dynamic Restart()
        {
            Program.Restart();
            return HttpStatusCode.OK;
        }

        private dynamic Run(string spec)
        {
            var environment = Request.Query.environment;

            var url = Context.Request.Url;

            var model = new SpecFlowScript(spec, url.SiteBase, environment);
            _controller.RunSpec(model, environment);

            var pageModel = PageDataModelBuilder.Build(spec);

            var vm = new RunnerViewModel(model, pageModel);

            return View["runner", vm];
        }

        private dynamic SendMessage()
        {
            var msg = ScriptEngine.Instance.GetMessage();

            var output = new RunnerOutput();
            output.Message = msg.Message.Replace(Environment.NewLine, "<BR/>");
            output.State = msg.State.ToString();
            return Response.AsJson(output);
        }
    }

    public struct RunnerOutput
    {
        public string Message;
        public string State;
    }
}
