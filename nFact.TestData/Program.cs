using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nFact.TestData
{
    class Program
    {
        static void Main(string[] args)
        {
            var testData = new TestData();
            var output = testData.Generate();

            if (args.Length < 1)
                return;

            var destPath = args[0];
            var destArtfacts = Path.Combine(destPath, "Artifacts");
            var sourceArtifacts = Path.Combine(output, "Artifacts");
            var projectXml = Path.Combine(destPath, "projects.xml");
            var sourceProjectXml = Path.Combine(output, "projects.xml");
            DeleteDirectory(destArtfacts);

            TestData.DirectoryCopy(sourceArtifacts, destArtfacts, true);
            File.Copy(sourceProjectXml, projectXml, true);
        }

        private static void DeleteDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
