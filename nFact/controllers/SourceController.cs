using System;
using System.Configuration;
using System.IO;

namespace nFact.controllers
{
    public class SourceController
    {
        public string GetTfsSourcePath(string spec)
        {
            // Get default path
            var sourcePath = ConfigurationManager.AppSettings["Source"];

            // Tfs source path is saved in temporary 'Source' file.
            var filePath = Path.Combine(Environment.CurrentDirectory, string.Format(@"projects\{0}\source", spec));

            if (File.Exists(filePath))
                sourcePath = File.ReadAllText(filePath);

            return sourcePath;
        }
    }
}