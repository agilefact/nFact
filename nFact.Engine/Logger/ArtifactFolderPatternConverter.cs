using System.IO;

namespace nFact.Engine.Logger
{
    public class ArtifactFolderPatternConverter : log4net.Util.PatternConverter
    {
        override protected void Convert(TextWriter writer, object state)
        {
            var artifacts = ScriptEngine.Instance.GetArtifacts();
            writer.Write(artifacts.FilePath);
        }
    }
}