namespace nFact.SpecFlow
{
    class Program
    {

        static void Main(string[] args)
        {
            
            var properties = new ReportProprties
                                 {
                                     SpecName = args[0],
                                     HtmlReport = args[1],
                                     TestRun = args[2],
                                     ArtifactsPath = args[3],
                                     Environment = args[4],
                                     Version = args[5]
                                 };
            /*
            var properties = new ReportProprties();
            properties.SpecName = "SpecFlowTest";
            properties.HtmlReport = "C:\\dev\\SpecReplay\\SpecReplay\\bin\\Debug\\Artifacts\\SpecFlowTest_24\\TestResult.html";
            properties.Server = "http://localhost:9999";
            properties.ArtifactsPath = "C:\\dev\\SpecReplay\\SpecReplay\\bin\\Debug\\Artifacts\\SpecFlowTest_24";
            properties.ArtifactsVersion = "SpecFlowTest_24";
             * */
            
            var generator = new ScenarioReplayReport(properties);
            generator.ModifyTitle();
            generator.AddReplayLinks();
            generator.AddExternalLinks();
            generator.ChangeFeaturesToStories();

        }
    }
}
