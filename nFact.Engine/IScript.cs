namespace nFact.Engine
{
    public interface IScript
    {
        string RootPath { get; }
        string Script { get; }
        string Spec { get; }
        string Environment { get; }
    }
}