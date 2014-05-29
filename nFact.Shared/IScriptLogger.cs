namespace nFact.Shared
{
    public interface IScriptLogger
    {
        void Log(string text);
        void LogError(string text);
    }
}