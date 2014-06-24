using System;
using System.Xml.Linq;

namespace nFact.Engine.Model
{
    public enum Result { Pending, Success, Failure }

    public class StoryResult
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Result Result { get; private set; }
        public bool Executed { get; private set; }
        public bool Success { get; private set; }
        public double Seconds { get; private set; }
        public int Asserts { get; private set; }
        public string Id { get; private set; }
        public bool Accepted { get; private set; }

        public static StoryResult Parse(XElement xml, string id)
        {
            Result r;
            bool b;
            int i;
            var s = new StoryResult();
            s.Id = id;

            s.Name = GetValue(xml, "name");
            s.Description = GetValue(xml, "description");

            Enum.TryParse(GetValue(xml, "result"), true, out r);
            s.Result = r;

            bool.TryParse(GetValue(xml, "executed"), out b);
            s.Executed = b;

            bool.TryParse(GetValue(xml, "success"), out b);
            s.Success = b;

            double secs;
            double.TryParse(GetValue(xml, "time"), out secs);  
            s.Seconds = secs;

            int.TryParse(GetValue(xml, "asserts"), out i);
            s.Asserts = i;

            bool.TryParse(GetValue(xml, "accepted"), out b);
            s.Accepted = b;

            return s;
        }

        private static string GetValue(XElement xml, string attribute)
        {
            return xml.Attribute(attribute) != null ? xml.Attribute(attribute).Value : string.Empty;
        }
    }
}