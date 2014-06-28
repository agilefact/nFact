using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using nFact.Engine;

namespace nFact.TestData
{
    public class TestData
    {
        DateTime _date = new DateTime(2014, 06, 1);
        private SpecManager _manager = new SpecManager();
        private string _pendingDir;
        private string _failureDir;
        private string _successDir;

        public string Generate()
        {
            var directory = Environment.CurrentDirectory;
            var templateDir = Path.Combine(directory, "TestRun");
            _pendingDir = Path.Combine(templateDir, "Pending");
            _failureDir = Path.Combine(templateDir, "Failure");
            _successDir = Path.Combine(templateDir, "Success");

            var dataDir = Path.Combine(directory, "Data");
            var artifactsDir = Path.Combine(dataDir, "Artifacts");
            CreateDirectory(dataDir);
            CreateDirectory(artifactsDir);

            var testPackageName = "SpecTests";
            var environment = "local";

            var testPackageArtifacts = Path.Combine(artifactsDir, testPackageName);
            CreateDirectory(testPackageArtifacts);

            IScript script = new Test(testPackageName, environment);

            var pendingCount = GetRandom(1, 5);
            var failCount = GetRandom(3, 7);
            var successCount = GetRandom(5, 8);

            CreateTestResults(TestResult.Pending, pendingCount, script, testPackageArtifacts);
            CreateTestResults(TestResult.Failure, failCount, script, testPackageArtifacts);
            CreateTestResults(TestResult.Success, successCount, script, testPackageArtifacts);

            var projectsFile = Path.Combine(dataDir, "projects.xml");
            SpecStore.SaveArtifacts(_manager.Model, projectsFile);
            return dataDir;
        }

        private void CreateTestResults(TestResult testResult, int totalTestRuns, IScript script, string testPackageArtifacts)
        {
            for (int i = 0; i < totalTestRuns; i++)
            {
                CreateTestRun(script, testPackageArtifacts, testResult);
            }
        }

        private void CreateTestRun(IScript script, string artifactsDir, TestResult result)
        {
            var artifacts = _manager.CreateArtifacts(script);
            artifacts.Date = GenerateTestDate();
            artifacts.RecordTestComplete();

            var testRunDir = Path.Combine(artifactsDir, artifacts.ArtifactsVersion);
            CreateDirectory(testRunDir);

            string sourcePath = _pendingDir;
            switch (result)
            {
                case TestResult.Pending:
                    sourcePath = _pendingDir;
                    break;
                case TestResult.Failure:
                    sourcePath = _failureDir;
                    break;
                case TestResult.Success:
                    sourcePath = _successDir;
                    break;
            }

            DirectoryCopy(sourcePath, testRunDir, false);
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private static void CreateDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }

            Directory.CreateDirectory(dir);
        }

        public int GetRandom(int lower, int upper)
        {
            var rnd = new Random();
            return rnd.Next(lower, upper);
        }

        public DateTime GenerateTestDate()
        {
            _date = _date.AddDays(1);
            var rnd = new Random();
            int hour = rnd.Next(9, 18);
            return _date.AddHours(hour);
        }
    }

    public enum TestResult {Pending, Failure, Success}

    public class Test : IScript
    {
        public Test (string project, string environment)
        {
            Spec = project;
            Environment = environment;
        }
        public string RootPath { get; private set; }
        public string Script { get; set; }
        public string Spec { get; private set; }
        public string Environment { get; private set; }
    }
}
