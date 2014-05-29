using System;
using System.Collections.Generic;
using System.IO;

namespace nFact.SpecFlow
{
    public class TestResultTextParser
    {
        private Dictionary<string, List<string>> _links = new Dictionary<string, List<string>>();

        private string _feature;

        public Dictionary<string, List<string>> GetLinks(string path)
        {
            using (var stream = new StreamReader(path))
            {
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    ParseFeaureLine(line);
                    ParseLinkLine(_feature, line);
                }
            }
            return _links;
        }

        private void ParseFeaureLine(string line)
        {
            if (!line.StartsWith("*****"))
                return;

            var text = line.Substring(6, line.Length - 6);
            var labels = text.Split('.');
            if (labels.Length < 2)
                return;

             
            // Pattern is: namespace.featureKey.scenarioKey
            // Note: namespace can have multiple levels;
            // Hance Feature key is found second from end.
            _feature = labels[labels.Length - 2];
        }

        private void ParseLinkLine(string feature, string line)
        {
            if (feature == null)
                return;

            if (!line.StartsWith("@link", StringComparison.InvariantCultureIgnoreCase)) 
                return;

            string link;
            if (GetContents(line, '{', '}', out link))
            {
                if (!_links.ContainsKey(feature))
                    _links.Add(feature, new List<string>());

                if (!_links[feature].Contains(link))
                    _links[feature].Add(link);
            }
        }

        private static bool GetContents(string text, char startMarker, char endMarker, out string contents)
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
