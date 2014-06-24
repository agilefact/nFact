using System.Diagnostics;
using System.IO;
using System.Linq;
using HtmlAgilityPack;

namespace nFact.SpecFlow
{
    public struct ReportProprties
    {
        public string SpecName;
        public string HtmlReport;
        public string TestRun;
        public string ArtifactsPath;
        public string Environment;
        public string Version;
    }

    public class ScenarioReplayReport
    {
        private readonly ReportProprties _properties;

        public ScenarioReplayReport(ReportProprties properties)
        {
            _properties = properties;
        }

        public void ModifyTitle()
        {
            var doc = new HtmlDocument();
            var report = _properties.HtmlReport;
            var environment = _properties.Environment;
            var version = _properties.Version;
            var library = string.Format("{0}.dll", _properties.SpecName);

            doc.Load(report);
            var headings = doc.DocumentNode.SelectNodes(@"//h2");
            var heading = headings.FirstOrDefault(h => h.InnerText == "Summary");
            if (heading == null)
                return;

            var parent = heading.ParentNode;

            var html = string.Format("<div>&nbsp;</div>");
            var br = HtmlNode.CreateNode(html);
            parent.InsertBefore(br, heading);

            if (!string.IsNullOrEmpty(environment))
            {
                html = string.Format("<div style='font-size: 14px; font-weight: bold'>Environment - {0}</div>", environment);
                var environmentHeading = HtmlNode.CreateNode(html);

                parent.InsertBefore(environmentHeading, heading);
            }

            html = string.Format("<div><span>Test Library: {0}</span></div>", library);
            var libNode = HtmlNode.CreateNode(html);

            parent.InsertBefore(libNode, heading);

            if (!string.IsNullOrEmpty(version))
            {
                html = string.Format("<div><span>Version: {0} </span><a href='@Log'>Log</a></div>", version);
                var versionNode = HtmlNode.CreateNode(html);

                parent.InsertBefore(versionNode, heading);
            }

            doc.Save(report);
        }

        public void ChangeFeaturesToStories()
        {
            var report = File.ReadAllText(_properties.HtmlReport);
            report = report.Replace("Features", "Stories");
            report = report.Replace("Feature", "Story");
            report = report.Replace("features", "stories");
            report = report.Replace("feature", "story");
            File.WriteAllText(_properties.HtmlReport, report);
        }

        public void AddReplayLinks()
        {
            var doc = new HtmlDocument();
            var report = _properties.HtmlReport;
            doc.Load(report);

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes(@"//span[@title]"))
            {
                var specName = _properties.SpecName;
                var title = node.Attributes["title"];

                // Scenario title is identified as beginning with the spec library namespace
                if (!title.Value.StartsWith(string.Format("{0}.", specName)))
                    continue;

                int scenarioCount;

                // Attempts to find next table row containing <pre> tag with scenario output.
                if (node.ParentNode.Name == "td" && 
                    node.ParentNode.ParentNode.Name == "tr")
                {
                    var nextSibling = node.ParentNode.ParentNode.NextSibling;
                    if (nextSibling.Name == "tr")
                    {
                        if (nextSibling.FirstChild.Name == "td" &&
                            nextSibling.FirstChild.FirstChild.Name == "div" &&
                            nextSibling.FirstChild.FirstChild.FirstChild.Name == "pre")
                        {
                            var scenarioOutput = nextSibling.FirstChild.FirstChild.FirstChild;
                            scenarioCount = ScenarioOutput.GetScenarioCount(scenarioOutput);
                            var setupLink = ScenarioOutput.GetTagContent(scenarioOutput, "@setup");
                            var tearDownLink = ScenarioOutput.GetTagContent(scenarioOutput, "@teardown");
                            


                            AddSetupLink(setupLink, node);
                            AddTeardownLink(tearDownLink, node);

                            if (scenarioCount == 0)
                                continue;

                            AddVideoLink(scenarioCount, node);
                            AddSlidesLink(scenarioCount, node);
                            AddStepsLink(scenarioCount, node);
                        }
                    }
                }
            }

            doc.Save(report);

            ChangeToggleFunction();
        }

        private void AddSetupLink(string setupLink, HtmlNode node)
        {
            AddLink(setupLink, node, "setup");
        }

        private void AddTeardownLink(string tearDownLink, HtmlNode node)
        {
            AddLink(tearDownLink, node, "teardown");
        }

        private void AddLink(string url, HtmlNode node, string label)
        {
            if (string.IsNullOrEmpty(url))
                return;

            var html = string.Format("<a href=\"{0}\" >[{1}]</a>", url, label);
            var link = HtmlNode.CreateNode(html);
            var space = HtmlNode.CreateNode("&nbsp;&nbsp;");
            node.ParentNode.AppendChild(space);
            node.ParentNode.AppendChild(link);
        }

        public void AddExternalLinks()
        {
            var filePath = Path.Combine(_properties.ArtifactsPath, "TestResult.txt");
            var links = TestResultTextParser.GetContent(filePath, "@link");
            var jiraIds = TestResultTextParser.GetContent(filePath, "@jira");
            var rallyIds = TestResultTextParser.GetContent(filePath, "@rally");

            var doc = new HtmlDocument();
            var report = _properties.HtmlReport;
            doc.Load(report);
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes(@"//span[@title]"))
            {
                if (node.ParentNode.Name != "h3")
                    continue;

                var title = node.Attributes["title"];
                if (title == null)
                    break;

                var feature = title.Value;
                if (links.ContainsKey(feature) && links[feature].Count > 0)
                {
                    var link = links[feature][0];
                    AddExternalLink(link, node);
                }

                if (jiraIds.ContainsKey(feature) && jiraIds[feature].Count > 0)
                {
                    var id = jiraIds[feature][0];
                    AddAcceptedButton(id, node);
                }

                if (rallyIds.ContainsKey(feature) && rallyIds[feature].Count > 0)
                {
                    var id = rallyIds[feature][0];
                    AddAcceptedButton(id, node);
                }
            }

            doc.Save(report);
        }

        private void AddAcceptedButton(string id, HtmlNode node)
        {
            var html = string.Format("<button data-bind=\"click: accept({0})\" class=\"btn btn-success btn-xs\" style=\"width: 60px\">Accept</button>", id);
            var linkNode = HtmlNode.CreateNode(html);
            var space = HtmlNode.CreateNode("&nbsp;&nbsp;");

            node.ParentNode.AppendChild(space);
            node.ParentNode.AppendChild(linkNode);
        }

        private void AddExternalLink(string link, HtmlNode node)
        {
            var html = string.Format("<a href=\"{0}\" target='_parent'>info>></a>", link);
            var linkNode = HtmlNode.CreateNode(html);
            var space = HtmlNode.CreateNode("&nbsp;&nbsp;");
            
            node.ParentNode.AppendChild(space);
            node.ParentNode.AppendChild(linkNode);
        }

        private void ChangeToggleFunction()
        {
            var report = File.ReadAllText(_properties.HtmlReport);
            report = report.Replace("class=\"hidden\"", "style = \"display: none;\"");
            File.WriteAllText(_properties.HtmlReport, report);
        }

        private void AddVideoLink(int scenarioCount, HtmlNode node)
        {
            var fileName = string.Format(@"Scen_{0}.mp4", scenarioCount);

            var artifactsPath = _properties.ArtifactsPath;
            var filePath = Path.Combine(artifactsPath, fileName);
            if (!File.Exists(filePath))
                return;

            var artifactsVersion = string.Format("TestRun_{0}", _properties.TestRun);
            var html =
                string.Format(
                    "<a href=\"#\" onclick=\"window.open('/assets/videoPlayer.html?file={0}&project={1}&artifacts={2}','targetWindow','toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=1000,height=800')\">[video]</a>",
                    fileName, _properties.SpecName, artifactsVersion);
            var link = HtmlNode.CreateNode(html);
            var space = HtmlNode.CreateNode("&nbsp;&nbsp;");
            node.ParentNode.AppendChild(space);
            node.ParentNode.AppendChild(link);
        }

        private void AddSlidesLink(int scenarioCount, HtmlNode node)
        {
            var spec = _properties.SpecName;
            var testRun = _properties.TestRun;

            var artifactsPath = _properties.ArtifactsPath;
            var slides = Path.Combine(artifactsPath, string.Format(@"artifacts\{0}", scenarioCount));
            if (!Directory.Exists(slides))
                return;

            var pics = Directory.GetFileSystemEntries(slides, "*.png");
            if (pics.Length == 0)
                return;

            var html =
                string.Format(
                    "<a href=\"#\" onclick=\"window.open('/{0}/{1}/{2}/slides','targetWindow','toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=1400,height=900')\">[slides]</a>",
                    spec, testRun, scenarioCount);
            var link = HtmlNode.CreateNode(html);
            var space = HtmlNode.CreateNode("&nbsp;&nbsp;");
            node.ParentNode.AppendChild(space);
            node.ParentNode.AppendChild(link);
        }

        private void AddStepsLink(int scenarioCount, HtmlNode node)
        {
            var fileName = string.Format(@"steps_{0}.zip", scenarioCount);

            var artifactsPath = _properties.ArtifactsPath;
            var filePath = Path.Combine(artifactsPath, fileName);
            if (!File.Exists(filePath))
                return;

            var unzipFile = UnZip.Exec(filePath, artifactsPath);

            File.Delete(filePath);

            var unzipFilePath = Path.Combine(artifactsPath, unzipFile);
            string stepsFile = string.Format("steps_{0}.mht", scenarioCount);
            var stepsFilePath = Path.Combine(artifactsPath, stepsFile);
            File.Move(unzipFilePath, stepsFilePath); //rename file

            var stepsReport = File.ReadAllText(stepsFilePath);
            stepsReport = stepsReport.Replace("Problem", "");
            stepsReport = stepsReport.Replace("problem", "");
            File.WriteAllText(stepsFilePath, stepsReport);

            var artifactsVersion = string.Format("{0}_{1}", _properties.SpecName, _properties.TestRun);
            var html =
                string.Format(
                    "<a href=\"#\" onclick=\"window.open('/artifacts/{0}/{1}','targetWindow','toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=1400,height=900')\">[steps]</a>",
                    artifactsVersion, stepsFile);
            var link = HtmlNode.CreateNode(html);
            var space = HtmlNode.CreateNode("&nbsp;&nbsp;");
            node.ParentNode.AppendChild(space);
            node.ParentNode.AppendChild(link);
        }
    }
}