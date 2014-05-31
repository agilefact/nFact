using System.Collections.Generic;

namespace nFact.Engine.Model
{
    public class TestEnvironment
    {
        public string Name { get; private set; }
        public int TestRuns { get; internal set; }
        public List<ProjectArtifacts> Artifacts { get; internal set; }

        public TestEnvironment(string name)
        {
            Name = name;
            Artifacts = new List<ProjectArtifacts>();
        }

        public void AddTestResults(ProjectArtifacts artifacts)
        {
            TestRuns++;
            Artifacts.Add(artifacts);
        }
    }
}