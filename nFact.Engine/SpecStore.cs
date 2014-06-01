using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using nFact.Engine.Model;

namespace nFact.Engine
{
    public class SpecStore
    {
        public static void LoadArtifacts(ProjectsModel dataModel)
        {
            LoadArtifacts("projects.xml", dataModel);
        }

        public static void LoadArtifacts(string file, ProjectsModel dataModel)
        {
            var artifactsFile = Path.Combine(Environment.CurrentDirectory, file);
            if (!File.Exists(artifactsFile))
                return;

            var data = XElement.Load(artifactsFile);
            foreach (var p in data.Descendants("Project"))
            {
                var project = new Project(p.GetValue<string>("Name"))
                                {
                                    TestRuns = p.GetValue<int>("TestRuns")
                                };

                foreach (var e in p.Descendants("TestEnvironment"))
                {
                    var environment = CreateTestEnvironment(e, project);
                    foreach (var a in e.Descendants("Artifacts"))
                    {
                        var artifacts = CreateProjectArtifacts(a, environment);
                        environment.Artifacts.Add(artifacts);
                    }
                    project.TestEnvironments.Add(environment.Name, environment);
                }

                dataModel.Projects.Add(project.Name, project);
            }
        }

        public static void SaveArtifacts(ProjectsModel dataModel)
        {
            var data = new XElement("Projects");
            foreach (var project in dataModel.Projects.Values)
            {
                var xProject = new XElement("Project");
                xProject.Add(new XAttribute("Name", project.Name));
                xProject.Add(new XAttribute("TestRuns", project.TestRuns));

                var xEnvironments = new XElement("TestEnvironments");

                foreach (var testEnvironment in project.TestEnvironments.Values)
                {
                    var xEnvironment = new XElement("TestEnvironment");
                    xEnvironment.Add(new XAttribute("Name", testEnvironment.Name));
                    xEnvironment.Add(new XAttribute("TestRuns", testEnvironment.TestRuns));

                    foreach (var artifacts in testEnvironment.Artifacts)
                    {
                        var xArtifacts = new XElement("Artifacts");
                        xArtifacts.Add(new XAttribute("Date", artifacts.Date.Date.ToString("yyyyMMdd")));
                        xArtifacts.Add(new XAttribute("Time", artifacts.Date.TimeOfDay.ToString("hhmmss")));
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

                        xEnvironment.Add(xArtifacts);
                    }

                    xEnvironments.Add(xEnvironment);
                }

                xProject.Add(xEnvironments);

                data.Add(xProject);
            }

            data.Save(Path.Combine(Environment.CurrentDirectory, "projects.xml"));
        }

        private static TestEnvironment CreateTestEnvironment(XElement x, Project project)
        {
            return new TestEnvironment(x.GetValue<string>("Name"), project)
                       {
                           TestRuns = x.GetValue<int>("TestRuns")
                       };
        }

        private static ProjectArtifacts CreateProjectArtifacts(XElement x, TestEnvironment environment)
        {
            DateTime date = x.GetValue<DateTime>("Date", "yyyyMMdd");
            var time = x.GetValue<TimeSpan>("Time", "hmmss");
            return new ProjectArtifacts(environment)
                       {
                           Date = date.Add(time),
                           TestRun = x.GetValue<int>("TestRun"),
                           Version = x.GetValue<string>("Version"),
                           NUnitResultTxtFile = x.GetValue<string>("ResultTxtFile"),
                           NUnitResultXmlFile = x.GetValue<string>("ResultXmlFile"),
                           SpecFlowResultFile = x.GetValue<string>("ResultHtmlFile")
                       };
        }
    }
}