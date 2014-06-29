using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using nFact.Engine;

namespace nFact.TestData
{
    public class EnvironmentSimulation
    {
        public string Name;
        public int PendingAttempts;
        public int FailureAttempts;
        public int SuccessAttempts;
        public bool Accepted;
        public IScript Script;
        public EnvironmentSimulation PrevEnvironment;
        public EnvironmentSimulation NextEnvironment;
        public TestResult LastResult;
        public bool IntegrationEnv;

        public static EnvironmentSimulation Create(string package, string environment, bool pending, bool integration)
        {
            var env = new EnvironmentSimulation();
            env.Name = environment;
            env.Script = new Test(package, environment);
            if (pending)
                env.PendingAttempts = TestData.GetRandom(1, 2);

            if (integration)
            {
                env.FailureAttempts = TestData.GetRandom(0, 1);
                env.SuccessAttempts = 1;
            }
            else
            {
                env.FailureAttempts = TestData.GetRandom(1, 5);
                env.SuccessAttempts = TestData.GetRandom(1, 3);
            }

            return env;
        }
    }
    public class TestData
    {
        DateTime _date = new DateTime(2014, 06, 1);
        private SpecManager _manager = new SpecManager();
        private string _pendingDir;
        private string _failureDir;
        private string _successDir;
        private int _failureAfterSuccess;

        private List<EnvironmentSimulation> _environments = new List<EnvironmentSimulation>();
        private string _testArtifacts;
        private EnvironmentSimulation _currentEnvironment;

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
                
            var packageName = "SpecTests";

            _testArtifacts = Path.Combine(artifactsDir, packageName);
            CreateDirectory(_testArtifacts);

            var local = SetupSimulation(packageName);

            while (!AllEnvironmentsAccepted())
            {
                RunTestSimulation(local);
            }


            Console.WriteLine("Failures after Success Count: " + _failureAfterSuccess);

            var projectsFile = Path.Combine(dataDir, "projects.xml");
            SpecStore.SaveArtifacts(_manager.Model, projectsFile);
            return dataDir;
        }

        private void RunTestSimulation(EnvironmentSimulation environment)
        {
            while (environment != null)
            {
                Simulate(environment);

                environment = environment.NextEnvironment;
            }
        }

        private void Simulate(EnvironmentSimulation environment)
        {
            string testResult = null;
            _currentEnvironment = environment;
            GenerateTestDate();
            while (environment.PendingAttempts > 0)
            {
                CreateTestRun(environment, TestResult.Pending);
                environment.PendingAttempts--;

                GenerateTestDate();
            }

            while (environment.FailureAttempts > 0)
            {
                CreateTestRun(environment, TestResult.Failure);
                environment.FailureAttempts--;

                GenerateTestDate();
                //SimulateAccepted(environment.PrevEnvironment, date);
            }
            
            while (environment.SuccessAttempts > 0)
            {
                
                var prevEnv = environment.PrevEnvironment;

                SimulateAccepted(prevEnv);
                if (AllPrevEnvironmentsPassed(environment))
                {
                    testResult = CreateTestRun(environment, TestResult.Success);
                    environment.SuccessAttempts--;
                    GenerateTestDate();
                }
                else
                {
                    GenerateTestDate(true);
                }
            }

            TestResultsManager.AcceptStoryResult("US39", testResult);

            Console.WriteLine("{0} Accepted", environment.Name);
            environment.Accepted = true;
        }

        private bool AllPrevEnvironmentsPassed(EnvironmentSimulation environment)
        {
            var prev = environment.PrevEnvironment;
            while (prev != null)
            {
                if (prev.LastResult != TestResult.Success)
                    return false;

                prev = prev.PrevEnvironment;
            }

            return true;
        }

        private void SimulateAccepted(EnvironmentSimulation environment)
        {
            if (environment == null)
                return;

            if (!environment.Accepted)
                return;

            var odds = GetRandom(0, 9);
            var result = TestResult.Success;
            if (odds < 3)
            {
                result = TestResult.Failure;
                _failureAfterSuccess++;
            }
            CreateTestRun(environment, result);

            SimulateAccepted(environment.PrevEnvironment);
        }

        private bool AllEnvironmentsAccepted()
        {
            return _environments.All(environment => environment.Accepted);
        }

        private EnvironmentSimulation SetupSimulation(string package)
        {
            var local = EnvironmentSimulation.Create(package, "local", true, false);
            var badev = EnvironmentSimulation.Create(package, "BADev", false, false);
            var t1 = EnvironmentSimulation.Create(package, "T1", false, true);
            var t2 = EnvironmentSimulation.Create(package, "T2", false, true);
            var stage = EnvironmentSimulation.Create(package, "Staging", false, true);
            
            local.NextEnvironment = badev;
            badev.PrevEnvironment = local;
            badev.NextEnvironment = t1;
            t1.PrevEnvironment = badev;
            t1.NextEnvironment = t2;
            t2.PrevEnvironment = t1;
            t2.NextEnvironment = stage;
            stage.PrevEnvironment = t2;

            _environments.Add(local);
            _environments.Add(badev);

            return local;
        }

        private string CreateTestRun(EnvironmentSimulation env, TestResult result)
        {
            return CreateTestRun(env, result, _date);
        }

        private string CreateTestRun(EnvironmentSimulation env, TestResult result, DateTime date)
        {
            var script = env.Script;

            var artifacts = _manager.CreateArtifacts(script);
            artifacts.Date = date;
            artifacts.RecordTestComplete();

            var testRunDir = Path.Combine(_testArtifacts, artifacts.ArtifactsVersion);
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

            env.LastResult = result;

            Console.WriteLine("{0}: {1} {2}", script.Environment, date.Day, result);

            DirectoryCopy(sourcePath, testRunDir, false);

            var testResult = Path.Combine(testRunDir, "TestResult.xml");
            return testResult;
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

        public static int GetRandom(int lower, int upper)
        {
            var rnd = new Random();
            return rnd.Next(lower, upper);
        }

        public void GenerateTestDate()
        {
            GenerateTestDate(_currentEnvironment.IntegrationEnv);
        }

        public void GenerateTestDate(bool nextDay)
        {
            var days = GetRandom(1, 5);
            if (nextDay)
                days = 1;

            _date = _date.AddDays(days);
            int hour = GetRandom(9, 18);
            _date.AddHours(hour);
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
