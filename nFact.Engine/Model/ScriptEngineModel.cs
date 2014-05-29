namespace nFact.Engine.Model
{
    public class ScriptEngineModel
    {
        public bool RecordVideo { get; set; }
        public bool RecordSteps { get; set; }

        public string[] Specs { get; set; }

        public ScriptEngineModel()
        {
            RecordVideo = true;
            Specs = new string[0];
        }
    }
}