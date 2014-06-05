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
        public Project GetStoryByEnvionment(string spec, string storyId)
        {
            var projectResults = GetProjectStoryResults(spec);
            var environments = from e in projectResults.Environments
                               select new Environment
                                          {
                                              Name = e.Name,
                                              Stories = e.Stories.Where(s => s.Id.Equals(storyId,
                                                                         StringComparison.InvariantCultureIgnoreCase)).ToArray()
                                          };

            return new Project
            {
                Name = spec,
                Environments = environments.ToArray()
            };
        }

        public Project GetProjectStoryResults(string spec)
        {
            var storyResults = GetStoryResults(spec);
            var environments = from r in storyResults
                               group r by r.Environment
                               into environemnts
                               select new Environment
                                          {
                                              Name = environemnts.Key,
                                              Stories = GetStories(environemnts)
                                          };
            return new Project
                       {
                           Name = spec,
                           Environments = environments.ToArray()
                       };
        }

        private static Story[] GetStories(IEnumerable<DataTransfer.Flat.StoryResult> environments)
        {
            var results = from e in environments
                          group e by e.Id into stories 
                          select new Story 
                          {
                              Id = stories.Key, 
                              Name = stories.Last().Name, 
                              Description = stories.Last().Description, 
                              Results = GetStoryResults(stories)
                          };

            return results.ToArray();
        }

        private static StoryResult[] GetStoryResults(IEnumerable<DataTransfer.Flat.StoryResult> stories)
        {
            var results = from s in stories
                               select new StoryResult
                                          {
                                              DurationSecs = s.DurationSecs, Result = s.Result, TestRun = s.TestRun, TestTime = s.TestTime, TestVersion = s.TestVersion
                                          };
            return results.ToArray();
        }

        private DataTransfer.Flat.StoryResult[] GetStoryResults(string spec)
        {
            var project = GetProject(spec);
            var results = from e in project.TestEnvironments.Values
                    from a in e.Artifacts
                    let storyResults = a.GetStoryResults()
                    from s in storyResults
                    select new DataTransfer.Flat.StoryResult
                    {
                        ProjectName = project.Name,
                        Environment = e.Name,
                        Description = s.Description,
                        DurationSecs = s.Seconds,
                        Id = s.Id,
                        Name = s.Name,
                        Result = s.Result.ToString(),
                        TestRun = a.TestRun,
                        TestTime = a.Date,
                        TestVersion = a.Version
                    };

            return results.ToArray();
        }
    }
}
