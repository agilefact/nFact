using System;

namespace nFact.DataTransfer
{
    public class Project
    {
        public string Name;
        public Environment[] Environments;
    }

    public class Environment
    {
        public string Name;
        public StoryResult[] StoryResults;
    }

    public class StoryResult
    {
        public int TestRun;
        public DateTime TestTime;
        public string TestVersion;
        public string Name;
        public string Description;
        public string Result;
        public double DurationSecs;
        public string Id;
    }
}