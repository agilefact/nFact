using System.Collections.Generic;

namespace nFact.Engine.Model
{
    public class ProjectsModel
    {
        public Dictionary<string, Project> Projects = new Dictionary<string, Project>();
        public Project CurrentProject;
    }
}