using System.Collections.Generic;

namespace nFact.Engine.Model
{
    public class Project
    {
        public string Name { get; private set; }
        public int TestRuns { get; internal set; }

        public List<ProjectArtifacts> Artifacts { get; internal set; }
        public Dictionary<string, TestEnvironment> TestEnvironments { get; internal set; }

        public Project(string name)
        {
            Name = name;
            Artifacts = new List<ProjectArtifacts>();
            TestEnvironments = new Dictionary<string, TestEnvironment>();
        }

        public ProjectArtifacts CreateArtifacts(string environment, string version)
        {
            var artifacts = ProjectArtifacts.Create(this, environment, version, TestRuns);

            Artifacts.Add(artifacts);
            return artifacts;
        }

        public void AddTestResults(string environment, ProjectArtifacts artifacts)
        {
            TestEnvironment testEnvironment;
            if (!TestEnvironments.ContainsKey(environment))
            {
                testEnvironment = new TestEnvironment(environment);
                TestEnvironments.Add(environment, testEnvironment);
            }
            else
            {
                testEnvironment = TestEnvironments[environment];
            }

            testEnvironment.AddTestResults(artifacts);
        }
    }
}