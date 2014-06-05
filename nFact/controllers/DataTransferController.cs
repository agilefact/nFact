
using nFact.Engine;
using nFact.Engine.Model.DataTransfer;

namespace nFact.controllers
{
    public class DataTransferController : CommandController
    {
        public Project GetStoryByEnvionment(string spec, string storyId)
        {
            var project = GetProject(spec);
            var manager = new StoryManager();
            return manager.GetStoryByEnvionment(project, storyId);
        }

        public Project GetProjectStoryResults(string spec)
        {
            var project = GetProject(spec);
            var manager = new StoryManager();
            return manager.GetProjectStoryResults(project);
        }
    }
}
