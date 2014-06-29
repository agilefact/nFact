﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using nFact.Engine.Model.DataTransfer;
using Environment = nFact.Engine.Model.DataTransfer.Environment;

namespace nFact.Engine
{
    public class StoryManager
    {
        public Project GetCurrentProjectStoryResults(Model.Project project)
        {
            var projectResults = GetResultsByEnvironment(project);
            var environments = from e in projectResults.Environments
                               select new Environment
                               {
                                   Name = e.Name,
                                   Stories = CurrentStoriesResults(e)
                               };

            return new Project
            {
                Name = project.Name,
                Environments = environments.ToArray()
            };
        }

        public Project GetCurrentProjectStoryResults(Model.Project project, string storyId)
        {
            var projectResults = GetResultsByEnvironment(project);
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

        private static Story[] CurrentStoriesResults(Environment e)
        {
            var stories = from s in e.Stories
                          select new Story
                          {
                              Id = s.Id,
                              Name = s.Name,
                              Description = s.Description,
                              Results = new[] { s.Results.Last() }
                          };
            return stories.ToArray();
        }

        public bool AcceptStoryResult(Model.Project project, string storyId, int testRun)
        {
            if (!CanAcceptStory(project, storyId, testRun)) 
                return false;

            var artifact = project.Artifacts.FirstOrDefault(a => a.TestRun == testRun);
            if (artifact == null)
                return false;

            var filePath = Path.Combine(artifact.FilePath, artifact.NUnitResultXmlFile);
            TestResultsManager.AcceptStoryResult(storyId, filePath);

            return true;
        }

        public bool DeclineStoryResult(Model.Project project, string storyId, int testRun)
        {
            var artifact = project.Artifacts.FirstOrDefault(a => a.TestRun == testRun);
            if (artifact == null)
                return false;

            var filePath = Path.Combine(artifact.FilePath, artifact.NUnitResultXmlFile);
            TestResultsManager.DeclineStoryResult(storyId, filePath);
            return true;
        }

        public bool CanAcceptStory(Model.Project project, string storyId, int testRun)
        {
            var storyResults = GetStoryResults(project);
            var story = storyResults.FirstOrDefault(s => s.Id == storyId && s.TestRun == testRun);
            if (story == null)
                return false;

            var storyPassed = story.Result.Equals("Success", StringComparison.InvariantCultureIgnoreCase);
            if (!storyPassed)
                return false;

            var environment = story.Environment;
            var futureStories = from s in storyResults
                                where s.Environment.Equals(environment, StringComparison.InvariantCultureIgnoreCase) &&
                                      s.Id.Equals(storyId, StringComparison.InvariantCultureIgnoreCase) &&
                                      s.TestRun >= testRun &&
                                      s.Accepted
                                select s;

            if (futureStories.Any())
                return false;
            return true;
        }

        public bool IsAccepted(Model.Project project, string storyId, int testRun)
        {
            var storyResults = GetStoryResults(project);
            var story = storyResults.FirstOrDefault(s => s.Id == storyId && s.TestRun == testRun);
            if (story == null)
                return false;

            return story.Accepted;
        }

        public Project GetResultsByStory(Model.Project project, string storyId)
        {
            var projectResults = GetResultsByStory(project);
            var stories = from s in projectResults.Stories
                          where s.Id == storyId
                          select s;

            return new Project
            {
                Name = project.Name,
                Stories = stories.ToArray()
            };
        }

        public Project GetResultsByEnvironment(Model.Project project, string storyId)
        {
            var projectResults = GetResultsByEnvironment(project);
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

        public Project GetResultsByStory(Model.Project project)
        {
            var storyResults = GetStoryResults(project);
            var stories = from r in storyResults   
                               group r by r.Id
                                   into s
                                   select new Story
                                   {
                                       Id = s.Key,
                                       Name = s.Last().Name,
                                       Description = s.Last().Description,
                                       Environments = GetEnvironmentsByStory(s)
                                   };
            return new Project
            {
                Name = project.Name,
                Stories = stories.ToArray()
            };
        }

        public Project GetResultsByEnvironment(Model.Project project)
        {
            var storyResults = GetStoryResults(project);
            var environments = from r in storyResults
                               group r by r.Environment
                                   into e
                                   select new Environment
                                   {
                                       Name = e.Key,
                                       Stories = GetStoriesByEnvironment(e)
                                   };
            return new Project
            {
                Name = project.Name,
                Environments = environments.ToArray()
            };
        }

        private static Environment[] GetEnvironmentsByStory(IEnumerable<Model.DataTransfer.Flat.StoryResult> stories)
        {
            var results = from s in stories
                          group s by s.Environment into e
                          select new Environment
                          {
                              Name = e.Key,
                              Results = GetStoryResults(e)
                          };

            return results.ToArray();
        }

        private static Story[] GetStoriesByEnvironment(IEnumerable<Model.DataTransfer.Flat.StoryResult> environments)
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
                              TestVersion = s.TestVersion,
                              Accepted = s.Accepted
                          };

            return results.ToArray();
        }

        public Model.DataTransfer.Flat.StoryResult[] GetStoryResults(Model.Project project)
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
                              TestVersion = a.Version,
                              Accepted = s.Accepted
                          };

            return results.ToArray();
        }
    }
}
