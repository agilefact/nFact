using System;
using System.IO;
using System.Linq;
using ScriptRunner;

namespace nFact.Engine.Model
{
    public class ProjectArtifacts : IArtifacts
    {
        public Project Project { get { return TestEnvironment.Project;   } }
        public string ProjectName { get { return Project.Name; } }
        public TestEnvironment TestEnvironment { get; private set; }
        public StoryResult[] StoryResults { get { return GetStoryResults(); } }
        public DateTime Date { get; set; }
        public int TestRun { get; set; }
        public string Version { get; set; }
        public string ArtifactsVersion { get { return string.Format("TestRun_{0}", TestRun); } }
        public string FilePath { get { return Path.Combine(ArtifactsDirectory, ArtifactsVersion); } }
        public string ArtifactsDirectory { 
            get
            {
                var path = Path.Combine(Environment.CurrentDirectory, "Artifacts");
                return Path.Combine(path, ProjectName);
            }
        }
        public string RelativeUrl { get { return string.Format("Artifacts/{0}/{1}", ProjectName, ArtifactsVersion); } }
        public string NUnitResultTxtFile = "TestResult.txt";
        public string NUnitResultXmlFile = "TestResult.xml";
        public string SpecFlowResultFile = "TestResult.html";
        public string[] ScenarioWMVFiles { get; set; }

        public ProjectArtifacts(TestEnvironment environment)
        {
            TestEnvironment = environment;
            Version = string.Empty;
            ScenarioWMVFiles = new string[0];
        }
        
        public static ProjectArtifacts Create(TestEnvironment environment, string version, int testRun)
        {
            return new ProjectArtifacts(environment)
                       {
                           Date = DateTime.Now, 
                           TestRun = testRun,
                           Version = version
                       };
        }

        public void DeleteExpiredArtifacts(int maxVersions)
        {
            if (maxVersions == 0)
                return;
            
            if (!Directory.Exists(ArtifactsDirectory))
                return;

            var directories = Directory.GetDirectories(ArtifactsDirectory);
            for(var testRun = TestRun - maxVersions; testRun >= 1; testRun --)
            {
                var artifactVersion = string.Format("TestRun_{0}", testRun);
                foreach (var directory in directories)
                {
                    if (directory.EndsWith(artifactVersion))
                        Directory.Delete(directory, true);
                }
                
            }
        }

        public string[] GetSlidesRelativeUrl(string scenario)
        {
            var scenarioUrl = string.Format("{0}/artifacts/{1}", RelativeUrl, scenario);
            var path = string.Format(@"{0}\artifacts\{1}", FilePath, scenario);
            var files = Directory.GetFileSystemEntries(path, "*.png");

            var slides = from f in files
                         orderby f
                         select string.Format("{0}/{1}", scenarioUrl, Path.GetFileName(f));

            return slides.ToArray();
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

        public StoryResult[] GetStoryResults()
        {
            var filePath = Path.Combine(FilePath, NUnitResultXmlFile);
            return TestResultsManager.GetStoryResults(filePath);
        }

        public void RecordTestComplete()
        {
            TestEnvironment.AddArtifacts(this);
        }
    }
}