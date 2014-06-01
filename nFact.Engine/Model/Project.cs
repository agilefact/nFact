using System.Collections.Generic;
using System.Linq;

namespace nFact.Engine.Model
{
    public class Project
    {
        public string Name { get; private set; }
        public int TestRuns { get; internal set; }

        public ProjectArtifacts[] Artifacts { get { return GetOrderedArtifacts(); } }
        public Dictionary<string, TestEnvironment> TestEnvironments { get; internal set; }

        public Project(string name)
        {
            Name = name;
            TestEnvironments = new Dictionary<string, TestEnvironment>();
        }

        public ProjectArtifacts[] GetOrderedArtifacts()
        {
            var orderedArtifacts = from a in TestEnvironments.Values.SelectMany(a => a.Artifacts)
                                   orderby a.Date
                                   select a;


            return orderedArtifacts.ToArray();
        }

        public ProjectArtifacts CreateArtifacts(string environment, string version)
        {
            TestEnvironment testEnvironment;
            if (!TestEnvironments.ContainsKey(environment))
            {
                testEnvironment = new TestEnvironment(environment, this);
                TestEnvironments.Add(environment, testEnvironment);
            }
            else
            {
                testEnvironment = TestEnvironments[environment]; 
            }

            return ProjectArtifacts.Create(testEnvironment, version, TestRuns);
        }
    }
}