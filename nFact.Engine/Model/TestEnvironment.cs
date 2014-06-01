using System.Collections.Generic;

namespace nFact.Engine.Model
{
    public class TestEnvironment
    {
        public Project Project { get; private set; }
        public string Name { get; private set; }
        public int TestRuns { get; internal set; }
        public List<ProjectArtifacts> Artifacts { get; internal set; }

        public TestEnvironment(string name, Project project)
        {
            Name = name;
            Project = project;
            Artifacts = new List<ProjectArtifacts>();
        }

        public void AddArtifacts(ProjectArtifacts artifacts)
        {
            TestRuns++;
            Artifacts.Add(artifacts);
        }
    }
}