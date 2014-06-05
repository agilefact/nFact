using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nFact.DataTransfer;
using Environment = nFact.DataTransfer.Environment;

namespace nFact.controllers
{
    public class DataTransferController : CommandController
    {
        public Project GetAllResultsByProject(string spec)
        {
            var project = GetProject(spec);
            if (project == null)
                throw new ApplicationException(String.Format("Could not find project '{0}'", spec));

            var result = new Project {Name = project.Name};
            var environments = new List<Environment>();
            foreach (var e in project.TestEnvironments.Values)
            {
                var environment = new Environment {Name = e.Name};
                var allStoryResults = new List<StoryResult>();
                foreach (var a in e.Artifacts)
                {
                    var storyResults = a.GetStoryResults();
                    foreach (var r in storyResults)
                    {
                        var sr = new StoryResult
                                     {
                                         Description = r.Description,
                                         DurationSecs = r.Seconds,
                                         Id = r.Id,
                                         Name = r.Name,
                                         Result = r.Result.ToString(),
                                         TestRun = a.TestRun,
                                         TestTime = a.Date,
                                         TestVersion = a.Version
                                     };
                        allStoryResults.Add(sr);
                    }
                }
                environment.StoryResults = allStoryResults.ToArray();
                environments.Add(environment);
            }
            result.Environments = environments.ToArray();
            return result;
        }
    }
}
