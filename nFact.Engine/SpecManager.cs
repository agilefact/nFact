using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using nFact.Engine.Model;

namespace nFact.Engine
{
    public class SpecManager
    {
        private readonly ProjectsModel _dataModel = new ProjectsModel();
        
        internal SpecManager(){}

        public void Init()
        {
            LoadArtifacts();
        }

        public void SetCurrentProject(string spec)
        {
            _dataModel.CurrentProject = GetProject(spec);
        }

        public Project GetCurrentProject()
        {
            return _dataModel.CurrentProject;
        }

        public string[] GetProjectSpecs()
        {
            var projectsPath = Path.Combine(Environment.CurrentDirectory, "projects");
            if (!Directory.Exists(projectsPath))
                return new string[0];

            var dirs = Directory.GetDirectories(projectsPath);
            return dirs.Select(d => new DirectoryInfo(d).Name).ToArray();
        }

        public void LoadArtifacts()
        {
            var artifactsFile = Path.Combine(Environment.CurrentDirectory, "projects.xml");
            if (!File.Exists(artifactsFile))
                return;

            var data = XElement.Load(artifactsFile);
            var projects = (from p in data.Descendants("Project")
                            let artifacts = from a in p.Descendants("Artifacts")
                                            select ProjectArtifacts(p.Attribute("Name").Value, a)
                            select new Project(p.Attribute("Name").Value)
                                       {
                                           TestRuns = int.Parse(p.Attribute("TestRuns").Value),
                                           Artifacts = artifacts.ToList()
                                       }).ToArray();

            foreach (var project in projects)
            {
                foreach (var artifact in project.Artifacts)
                {
                    artifact.Project = project;
                }
            }

            _dataModel.Projects = projects.ToDictionary(p => p.Name, p => p);
        }

        private static ProjectArtifacts ProjectArtifacts(string project, XElement a)
        {
            var resultTxt = a.Attribute("ResultTxtFile");
            var resultXml = a.Attribute("ResultXmlFile");
            var resultHtml = a.Attribute("ResultHtmlFile");
            var date = GetValue(a, "Date");
            var testRun = GetValue(a, "TestRun");
            var version = GetValue(a, "Version");
            return new ProjectArtifacts
                       {
                           Date = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture),
                           TestRun = int.Parse(testRun),
                           Version = version,
                           ProjectName = project,
                           NUnitResultTxtFile = resultTxt == null ? string.Empty : resultTxt.Value,
                           NUnitResultXmlFile = resultXml == null ? string.Empty : resultXml.Value,
                           SpecFlowResultFile = resultHtml == null ? string.Empty : resultHtml.Value
                       };
        }

        public static string GetValue(XElement a, string name)
        {
            var atrribute = a.Attribute(name);
            return atrribute == null ? null : atrribute.Value;
        }

        public void SaveArtifacts()
        {
            var data = new XElement("Projects");
            foreach (var project in _dataModel.Projects.Values)
            {
                var xProject = new XElement("Project");
                xProject.Add(new XAttribute("Name", project.Name));
                xProject.Add(new XAttribute("TestRuns", project.TestRuns));

                foreach (var artifacts in project.Artifacts)
                {
                    var xArtifacts = new XElement("Artifacts");
                    xArtifacts.Add(new XAttribute("Date", artifacts.Date.Date.ToString("yyyyMMdd")));
                    xArtifacts.Add(new XAttribute("TestRun", artifacts.TestRun));
                    if (artifacts.Version != null)
                        xArtifacts.Add(new XAttribute("Version", artifacts.Version));
                    
                    if (artifacts.NUnitResultTxtFile != null)
                        xArtifacts.Add(new XAttribute("ResultTxtFile", artifacts.NUnitResultTxtFile));

                    if (artifacts.NUnitResultXmlFile != null)
                        xArtifacts.Add(new XAttribute("ResultXmlFile", artifacts.NUnitResultXmlFile));

                    if (artifacts.SpecFlowResultFile != null)
                        xArtifacts.Add(new XAttribute("ResultHtmlFile", artifacts.SpecFlowResultFile));

                    foreach (string file in artifacts.ScenarioWMVFiles)
                    {
                        var xFile = new XElement("WmvFile");
                        xFile.Add(new XAttribute("Name", file));
                        xArtifacts.Add(xFile);
                    }

                    xProject.Add(xArtifacts);
                }

                data.Add(xProject);
            }

            data.Save(Path.Combine(Environment.CurrentDirectory, "projects.xml"));
        }

        public ProjectArtifacts CreateArtifacts(string projectSpecName, string version)
        {
            var project = GetProject(projectSpecName);
            project.TestRuns++;
            var artifacts =  project.CreateArtifacts(version);
            
            return artifacts;
        }

        public ProjectArtifacts GetArtifacts(string projectSpecName)
        {
            var project = GetProject(projectSpecName);
            return project.Artifacts.FirstOrDefault(p => p.TestRun == project.TestRuns);
        }

        public ProjectArtifacts GetArtifacts(string projectSpecName, int testRun)
        {
            var project = GetProject(projectSpecName);
            return project.Artifacts.FirstOrDefault(p => p.TestRun == testRun);
        }

        public Project GetProject(string projectSpecName)
        {
            var projects = _dataModel.Projects;

            if (!projects.ContainsKey(projectSpecName))
            {
                var newProject = new Project(projectSpecName);
                projects.Add(projectSpecName, newProject);
                return newProject;
            }

            var project = projects[projectSpecName];
            return project;
        }
    }
}