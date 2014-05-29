namespace nFact.viewModels
{
    public class SlidesViewModel
    {
        public string[] Images { get; private set; }

        public SlidesViewModel(string[] images)
        {
            Images = images;
        }
    }
}
