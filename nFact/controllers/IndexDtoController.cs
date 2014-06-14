
using nFact.Engine;
using nFact.Engine.Model.DataTransfer;

namespace nFact.controllers
{
    public class IndexDtoController : IndexController
    {
        public Project GetCurrentProjectStoryResults(string spec, string storyId)
        {
            var project = GetProject(spec);
            var manager = new StoryManager();
            return manager.GetCurrentProjectStoryResults(project, storyId);
        }

        public Project GetProjectStoryResults(string spec, string storyId)
        {
            var project = GetProject(spec);
            var manager = new StoryManager();
            return manager.GetProjectStoryResults(project, storyId);
        }

        public Project GetProjectStoryResults(string spec)
        {
            var project = GetProject(spec);
            var manager = new StoryManager();
            return manager.GetProjectStoryResults(project);
        }
    }
}
