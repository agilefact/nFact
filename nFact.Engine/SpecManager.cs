using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using nFact.Engine.Model;

namespace nFact.Engine
{
    public class SpecManager
    {
        private readonly ProjectsModel _dataModel = new ProjectsModel();

        internal SpecManager()
        {
        }

        public void Init()
        {
            SpecStore.LoadArtifacts(_dataModel);
        }

        public void LoadArtifacts(string file)
        {
            SpecStore.LoadArtifacts(file, _dataModel);
        }

        public void SaveArtifacts()
        {
            SpecStore.SaveArtifacts(_dataModel);
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

        public ProjectArtifacts CreateArtifacts(IScript script)
        {
            var spec = script.Spec;
            var version = GetProjectDllVersion(spec);
            var project = GetProject(spec);
            if (project == null)
                project = CreateProject(spec);

            project.TestRuns++;
            var environment = script.Environment;
            var artifacts = project.CreateArtifacts(environment, version);

            return artifacts;
        }

        public ProjectArtifacts[] GetAllArtifacts(string projectSpecName)
        {
            var project = GetProject(projectSpecName);
            return project.Artifacts;
        }

        public ProjectArtifacts GetLatestArtifacts(string projectSpecName)
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
                return null;
            }

            var project = projects[projectSpecName];
            return project;
        }

        public Project CreateProject(string projectSpecName)
        {
            var projects = _dataModel.Projects;

            if (projects.ContainsKey(projectSpecName))
                throw new ApplicationException(string.Format("Cannot create new project '{0}', as there already exists one with this name."));

            var newProject = new Project(projectSpecName);
            projects.Add(projectSpecName, newProject);
            return newProject;
        }

        internal static string GetProjectDllVersion(string spec)
        {
            var binPath = Path.Combine(Environment.CurrentDirectory, "projects");
            binPath = Path.Combine(binPath, spec);
            var defaultBinPath = ConfigurationManager.AppSettings["DefaultProjectBinPath"];
            binPath = Path.Combine(binPath, defaultBinPath);
            binPath = Path.Combine(binPath, string.Format("{0}.dll", spec));
            var projectAssembly = Assembly.LoadFile(binPath);

            var assemblyName = projectAssembly.GetName();
            var version = assemblyName.Version.ToString();
            return version;
        }
    }
}

public static class XElementExtension
{
    public static dynamic GetValue<T>(this XElement x, string name)
    {
        return GetValue<T>(x, name, null);
    }

    public static dynamic GetValue<T>(this XElement x, string name, string format)
    {   
        var attribute = x.Attribute(name);

        if (typeof(T) == typeof(string))
            return attribute == null ? null : attribute.Value;

        if (typeof(T) == typeof(int))
        {
            var i = 0;
            if (attribute != null)
                int.TryParse(attribute.Value, out i);
            return i;
        }   
        if (typeof(T) == typeof(DateTime) && !string.IsNullOrEmpty(format))
        {
            if (attribute == null)
                return DateTime.MinValue;

            return DateTime.ParseExact(attribute.Value, format, CultureInfo.InvariantCulture);
        }

        if (typeof(T) == typeof(TimeSpan) && !string.IsNullOrEmpty(format))
        {
            if (attribute == null)
                return TimeSpan.MinValue;

            return TimeSpan.ParseExact(attribute.Value, format, CultureInfo.InvariantCulture);
        }

        return null;
    }
}
