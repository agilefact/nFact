using nFact.Shared;

namespace nFact.Media
{
    public class Recorders
    {
        public IRecorder VideoRecorder { get; set; }
        public IRecorder SlidesRecorder { get; set; }

        public static Recorders Create()
        {
            return new Recorders {VideoRecorder = new VideoRecorder(), SlidesRecorder = new SlidesRecorder()};
        }
    }
}