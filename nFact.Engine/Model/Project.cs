using System.Collections.Generic;

namespace nFact.Engine.Model
{
    public class Project
    {
        public int TestRuns { get; internal set; }
        public string Name { get; private set; }

        public List<ProjectArtifacts> Artifacts { get; internal set; }

        public Project(string name)
        {
            Name = name;
            Artifacts = new List<ProjectArtifacts>();
        }

        public ProjectArtifacts CreateArtifacts(string version)
        {
            var artifacts = ProjectArtifacts.Create(this, TestRuns, version);

            Artifacts.Add(artifacts);
            return artifacts;
        }
    }
}