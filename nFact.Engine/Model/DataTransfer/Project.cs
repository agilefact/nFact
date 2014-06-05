using System;

namespace nFact.Engine.Model.DataTransfer.Flat
{
    public class StoryResult
    {
        public string ProjectName;
        public string Environment;
        public string Name;
        public string Description;
        public string Id;
        public int TestRun;
        public DateTime TestTime;
        public string TestVersion;
        public string Result;
        public double DurationSecs;
    }
}

namespace nFact.Engine.Model.DataTransfer
{
    public class Project
    {
        public string Name;
        public Environment[] Environments;
    }

    public class Environment
    {
        public string Name;
        public Story[] Stories;
    }

    public class Story
    {
        public string Name;
        public string Description;
        public string Id;
        public StoryResult[] Results;
    }

    public class StoryResult
    {
        public int TestRun;
        public DateTime TestTime;
        public string TestVersion;
        public string Result;
        public double DurationSecs;
    }
}