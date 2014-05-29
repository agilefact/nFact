using System.IO;
using nFact.Engine;

namespace nFact
{
    public class SpecFlowScript : IScript
    {
        public string RootPath { get; private set; }
        public string Script { get; private set; }
        public string Spec { get; private set; }
        public string Host { get; private set; }
        public string Environment { get; private set; }

        public SpecFlowScript(string spec, string host, string environment)
        {
            Spec = spec;
            Host = host;
            RootPath = System.Environment.CurrentDirectory;
            Script = Path.Combine(System.Environment.CurrentDirectory, @"scripts\runspecflow.ps1");
            Environment = environment;
        }
    }
}
