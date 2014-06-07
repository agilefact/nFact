using System.IO;
using System.Text;
using HtmlAgilityPack;
using nFact.Shared;

namespace nFact.SpecFlow
{
    public class ScenarioOutput
    {
        /// <summary>
        /// Loops through scenario output to find scenario count. Removes ScenarioStart and ScenarioEnd markers.
        /// </summary>
        /// <param name="scenarioOutput"></param>
        /// <returns>0 if cannot find scenario count.</returns>
        public static int GetScenarioCount(HtmlNode scenarioOutput)
        {
            int scenarioCount = 0;
            var output = new StringBuilder();
            using (var reader = new StringReader(scenarioOutput.InnerText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string command, parameter;
                    CommandParser.GetCommand(line, out command, out parameter);
                    var count = 0;
                    if (IsScenarioStart(command, parameter, out count))
                    {
                        if (count > 0)
                            scenarioCount = count;

                        continue;
                    }
                    
                    if (IsScenarioEnd(command))
                        continue;

                    output.AppendLine(line);
                }
            }
            scenarioOutput.InnerHtml = output.ToString();
            return scenarioCount;
        }

        private static bool IsScenarioStart(string command, string parameter, out int scenarioCount)
        {
            scenarioCount = 0;
            if (command == null)
                return false;

            if (command.StartsWith(CommandParser.ScenarioStart))
            {
                int.TryParse(parameter, out scenarioCount);
                return true;
            }
            return false;
        }

        private static bool IsScenarioEnd(string command)
        {
            return command != null && command.StartsWith(CommandParser.ScenarioEnd);
        }

        public static string GetTagContent(HtmlNode scenarioOutput, string tag)
        {
            string tagContent = null;
            var output = new StringBuilder();
            using (var reader = new StringReader(scenarioOutput.InnerText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var content = TestResultTextParser.ParseTagLine(line, tag);
                    if (content != null)
                    {
                        tagContent = content;
                        continue;
                    }

                    output.AppendLine(line);
                }
            }
            scenarioOutput.InnerHtml = output.ToString();
            return tagContent;
        }
    }
}