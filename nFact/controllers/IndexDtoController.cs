
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

        public Project GetProjectStoryResults(string spec, string storyId)
        {
            var project = GetProject(spec);
            return _manager.GetProjectStoryResults(project, storyId);
        }

        public Project GetProjectStoryResults(string spec)
        {
            var project = GetProject(spec);
            return _manager.GetProjectStoryResults(project);
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
