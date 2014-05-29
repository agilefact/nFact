namespace nFact.Media
{
    public class ConvertJob
    {
        public string SourceFile;
        public string DestFile;

        public ConvertJob(string sourceFile, string destFile)
        {
            SourceFile = sourceFile;
            DestFile = destFile;
        }
    }
}