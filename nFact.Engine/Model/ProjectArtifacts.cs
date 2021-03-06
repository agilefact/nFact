using System;
using System.IO;
using System.Linq;
using ScriptRunner;

namespace nFact.Engine.Model
{
    public class ProjectArtifacts : IArtifacts
    {
        public Project Project { get; internal set; }
        public string ProjectName { get; set; }
        public DateTime Date { get; set; }
        public int TestRun { get; set; }
        public string Version { get; set; }
        public string ArtifactsVersion { get { return string.Format("{0}_{1}", ProjectName, TestRun); } }
        public string FilePath
        {
            get
            {
                var path = string.Format(@"Artifacts\{0}", ArtifactsVersion);
                return Path.Combine(Environment.CurrentDirectory, path);
            }
        }

        public string RelativeUrl { get { return string.Format("Artifacts/{0}", ArtifactsVersion); } }

        public string NUnitResultTxtFile { get; set; }
        public string NUnitResultXmlFile { get; set; }
        public string SpecFlowResultFile { get; set; }
        public string[] ScenarioWMVFiles { get; set; }

        public ProjectArtifacts()
        {
            Version = string.Empty;
            ScenarioWMVFiles = new string[0];
        }

        public static ProjectArtifacts Create(Project project, int testRun, string version)
        {
            return new ProjectArtifacts
                       {
                           Project = project,
                           ProjectName = project.Name, 
                           Date = DateTime.Now, 
                           TestRun = testRun,
                           Version = version
                       };
        }

        public string[] GetSlidesRelativeUrl(string scenario)
        {
            var scenarioUrl = string.Format("{0}/artifacts/{1}", RelativeUrl, scenario);
            var path = string.Format(@"{0}\artifacts\{1}", FilePath, scenario);
            var files = Directory.GetFileSystemEntries(path, "*.png");

            var slides = from f in files
                         select string.Format("{0}/{1}", scenarioUrl, Path.GetFileName(f));

            return slides.ToArray();
        }

        public void CheckFiles()
        {
            var txtFile = NUnitResultTxtFile;
            var xmlFile = NUnitResultXmlFile;
            var htmlFile = SpecFlowResultFile;
            CheckFile("TestResult.txt", out txtFile);
            CheckFile("TestResult.xml", out xmlFile);
            CheckFile("TestResult.html", out htmlFile);
            NUnitResultTxtFile = txtFile;
            NUnitResultXmlFile = xmlFile;
            SpecFlowResultFile = htmlFile;
        }

        private void CheckFile(string fileName, out string attribute)
        {
            if (File.Exists(Path.Combine(FilePath, fileName)))
            {
                attribute = fileName;
                return;
            }

            attribute = null;
        }

        public void Delete()
        {
            Directory.Delete(FilePath, true);
        }
    }
}