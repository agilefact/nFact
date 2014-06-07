
using System;
using System.Collections.Generic;
using System.Linq;
using nFact.Engine.Model.DataTransfer;
using Environment = nFact.Engine.Model.DataTransfer.Environment;

namespace nFact.Engine
{
    public class StoryManager
    {
        public Project GetCurrentProjectStoryResults(Model.Project project, string storyId)
        {
            var projectResults = GetProjectStoryResults(project);
            var environments = from e in projectResults.Environments
                               select new Environment
                               {
                                   Name = e.Name,
                                   Stories = CurrentStoriesResults(storyId, e)
                               };

            return new Project
            {
                Name = project.Name,
                Environments = environments.ToArray()
            };
        }

        private static Story[] CurrentStoriesResults(string storyId, Environment e)
        {
            var stories = from s in e.Stories
                         where s.Id.Equals(storyId, StringComparison.InvariantCultureIgnoreCase)
                         select new Story
                                    {
                                        Id = s.Id,
                                        Name = s.Name,
                                        Description = s.Description,
                                        Results = new[] {s.Results.Last()}
                                    };
            return stories.ToArray();
        }

        public Project GetProjectStoryResults(Model.Project project, string storyId)
        {
            var projectResults = GetProjectStoryResults(project);
            var environments = from e in projectResults.Environments
                               select new Environment
                               {
                                   Name = e.Name,
                                   Stories = e.Stories.Where(s => s.Id.Equals(storyId,
                                                              StringComparison.InvariantCultureIgnoreCase)).ToArray()
                               };

            return new Project
            {
                Name = project.Name,
                Environments = environments.ToArray()
            };
        }

        public Project GetProjectStoryResults(Model.Project project)
        {
            var storyResults = GetStoryResults(project);
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
                Name = project.Name,
                Environments = environments.ToArray()
            };
        }

        private static Story[] GetStories(IEnumerable<Model.DataTransfer.Flat.StoryResult> environments)
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

        private static StoryResult[] GetStoryResults(IEnumerable<Model.DataTransfer.Flat.StoryResult> stories)
        {
            var results = from s in stories
                          select new StoryResult
                          {
                              DurationSecs = s.DurationSecs,
                              Result = s.Result,
                              TestRun = s.TestRun,
                              TestTime = s.TestTime,
                              TestVersion = s.TestVersion
                          };

            return results.ToArray();
        }

        private Model.DataTransfer.Flat.StoryResult[] GetStoryResults(Model.Project project)
        {
            var results = from e in project.TestEnvironments.Values
                          from a in e.Artifacts
                          let storyResults = a.GetStoryResults()
                          from s in storyResults
                          select new Model.DataTransfer.Flat.StoryResult
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
