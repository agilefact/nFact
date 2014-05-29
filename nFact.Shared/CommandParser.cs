namespace nFact.Shared
{
    public class CommandParser
    {
        public const string ScenarioStart = "Scenario.Start";
        public const string ScenarioEnd = "Scenario.End";

        public static void GetCommand(string text, out string command, out string parameter)
        {
            string cmd;
            string param = null;
            if (GetContents(text, '{', '}', out cmd))
            {
                GetContents(cmd, '(', ')', out param);
            }
            command = cmd;
            parameter = param;
        }

        public static bool GetContents(string text, char startMarker, char endMarker, out string contents)
        {
            contents = null;
            var startIndex = text.IndexOf(startMarker);
            var endIndex = text.IndexOf(endMarker);
            if (startIndex == -1 || endIndex == -1)
                return false;

            if (endIndex < startIndex)
                return false;

            var length = endIndex - startIndex;
            contents = text.Substring(startIndex + 1, length - 1);

            return true;
        }   
    }
}