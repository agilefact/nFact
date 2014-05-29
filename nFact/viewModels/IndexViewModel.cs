using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using nFact.Engine.Model;

namespace nFact.viewModels
{
    public class IndexViewModel : PageViewModel
    {
        public string ReportHeader { get; set; }
        public string ReportBody { get; set; }

        private readonly string _spec;
        private ProjectArtifacts _artifacts;

        public IndexViewModel(PageDataModel pageModel) : base(pageModel)
        {
            _spec = pageModel.selectedSpec;
        }

        public void RenderReport(ProjectArtifacts artifacts)
        {
            _artifacts = artifacts;

            SetTestRunPagenation(artifacts.TestRun, artifacts.Project.TestRuns);

            var resultFile = artifacts.SpecFlowResultFile;
            if (resultFile == null)
                return;

            var reportPath = Path.Combine(artifacts.FilePath, resultFile);
            if (!File.Exists(reportPath)) 
                return;

            var htmlReport = LoadSpecFlowReport(reportPath);
            ModifySpecFlowReport(htmlReport);
            Controls.HideControls();
        }

        private void ModifySpecFlowReport(HtmlDocument htmlReport)
        {
            AddTitle(htmlReport);
            AddLogLink(htmlReport);
            Merge(htmlReport);
        }

        private void AddTitle(HtmlDocument htmlReport)
        {
            var nodes = htmlReport.DocumentNode.SelectNodes("//h1");
            var heading = nodes.FirstOrDefault(n => n.InnerText.Contains(_spec));
            if (heading == null)
                return;

            heading.InnerHtml = string.Format("{0} Report", _spec);

            var headingPath = Path.Combine(Environment.CurrentDirectory, @"assets\ReportTitle.html");
            var headingHtml = File.ReadAllText(headingPath);
            headingHtml = headingHtml.Replace("@Title", string.Format("{0} Report", _spec));

            var headingTemp = HtmlNode.CreateNode(headingHtml);
            heading.ParentNode.ReplaceChild(headingTemp, heading);
        }

        private void AddLogLink(HtmlDocument htmlReport)
        {
            var log = string.Format("/{0}/{1}/artifacts/log.txt", _spec, _artifacts.TestRun);
            htmlReport.DocumentNode.InnerHtml = htmlReport.DocumentNode.InnerHtml.Replace("@Log", log);
        }

        private void Merge(HtmlDocument htmlReport)
        {
            var reportBody = htmlReport.DocumentNode.SelectSingleNode("//body");
            var reportHeader = htmlReport.DocumentNode.SelectSingleNode("//head");

            var reportTitle = reportHeader.SelectSingleNode("//title");
            reportHeader.RemoveChild(reportTitle);

            ReportHeader = reportHeader.InnerHtml;
            ReportBody = reportBody.InnerHtml;
        }

        private static HtmlDocument LoadSpecFlowReport(string specFlowReport)
        {
            var htmlReport = new HtmlDocument();
            htmlReport.Load(specFlowReport);
            return htmlReport;
        }
    }
}
