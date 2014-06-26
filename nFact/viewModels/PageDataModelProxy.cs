namespace nFact.viewModels
{
    internal class PageDataModelProxy : PageDataModel
    {
        public new string video { get; set; }
        public new string steps { get; set; }
        public new string testRun { get; set; }

        public PageDataModel Instance()
        {
            var t = 0;
            return new PageDataModel
                       {
                           video = bool.Parse(video), 
                           steps = bool.Parse(steps),
                           selectedSpec = selectedSpec,
                           specs = specs,
                           selectedEnvironment = selectedEnvironment,
                           testRun = t
                       };
        }
    }
}