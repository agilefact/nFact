
using System;
using nFact.Engine;
using nFact.Engine.Model.DataTransfer;

namespace nFact.controllers
{
    public class IndexDtoController : IndexController
    {
        private readonly StoryManager _manager = new StoryManager();

        public Project GetCurrentProjectStoryResults(string spec, string storyId)
        {
            var project = GetProject(spec);
            if (string.IsNullOrEmpty(storyId))
                return _manager.GetCurrentProjectStoryResults(project);

            return _manager.GetCurrentProjectStoryResults(project, storyId);
        }

        public Project GetResultsByEnvironment(string spec, string storyId)
        {
            var project = GetProject(spec);
            return _manager.GetResultsByEnvironment(project, storyId);
        }

        public Project GetResultsByEnvironment(string spec)
        {
            var project = GetProject(spec);
            return _manager.GetResultsByEnvironment(project);
        }

        public Project GetResultsByStory(string spec)
        {
            var project = GetProject(spec);
            return _manager.GetResultsByStory(project);
        }

        public Project GetResultsByStory(string spec, string storyId)
        {
            var project = GetProject(spec);
            return _manager.GetResultsByStory(project, storyId);
        }

        public bool AcceptStory(string spec, string storyId, int testRun)
        {
            var project = GetProject(spec);
            return _manager.AcceptStoryResult(project, storyId, testRun);
        }

        public bool CanAcceptStory(Engine.Model.Project project, string storyId, int testRun)
        {
            return _manager.CanAcceptStory(project, storyId, testRun);
        }
    }
}
