using System;
using System.Collections.Generic;
using System.Linq;
using nFact.Engine;
using nFact.Engine.Model.DataTransfer;

namespace nFact.controllers
{
    public class ChartController : IndexDtoController
    {
        public StoryCycleDuration GetStoryCycleTime(string spec)
        {
            var project = GetProject(spec);
            var manager = new StoryManager();
            var storyResults = manager.GetStoryResults(project);
            var stories = from r in storyResults
                          group r by r.Id
                              into s
                              select new
                              {
                                  Id = s.Key,
                                  Results = s.ToArray()
                              };

            var storyIds = new List<string>();

            var deploymentCycle = new EnvironmentCycle() {name = "Deployment Overhead"};
            var storyDurations = new List<CycleDuration>();
            foreach (var story in stories)
            {
                storyIds.Add(story.Id);
                var fistTestRunNum = story.Results.Min(r => r.TestRun);
                var lastTestRunNum = story.Results.Max(r => r.TestRun);
                var startDate = story.Results.First(r => r.TestRun == fistTestRunNum).TestTime;
                var endDate = story.Results.First(r => r.TestRun == lastTestRunNum).TestTime;
                var duration = new CycleDuration();
                duration.start = startDate;
                duration.end = endDate;
                duration.days = endDate.Subtract(startDate).Days;
                duration.enableAnnotation = true;
                duration.annotation =  duration.days + " Pts";
                storyDurations.Add(duration);
            }


            deploymentCycle.CycleDurations = storyDurations.ToArray();

            var developmentCycle = GetDevCycleTime(storyDurations);
            return new StoryCycleDuration
                       {
                           stories = storyIds.ToArray(),
                           environmentCycle = new[] {developmentCycle, deploymentCycle}
                       };
        }

        private EnvironmentCycle GetDevCycleTime(IEnumerable<CycleDuration> deploymentDurations)
        {
            var cycles = new List<CycleDuration>();
            foreach (var deploymentDuration in deploymentDurations)
            {
                var rnd = new Random();
                var days = rnd.Next(5, 15);
                var cycleDuration = new CycleDuration
                                        {
                                            start = deploymentDuration.start.Subtract(TimeSpan.FromDays(days)),
                                            end = deploymentDuration.start
                                        };
                cycles.Add(cycleDuration);
            }
            
            return new EnvironmentCycle
                       {
                           name = "Development",
                           CycleDurations = cycles.ToArray()
                       };
        }

        public StoryCycleDuration GetDeploymentCycleTime(string spec)
        {
            var results = GetResultsByStoryEnvironment(spec);
            return StoryCycleTime(results);
        }

        public StoryCycleDuration GetDeploymentCycleTime(string spec, string storyId)
        {
            var results = GetResultsByStoryEnvironment(spec, storyId);
            return StoryCycleTime(results);
        }

        private static StoryCycleDuration StoryCycleTime(Project results)
        {
            var storyIds = new List<string>();
            var cycles = new Dictionary<string, EnvironmentCycleProxy>();

            var storyName = string.Empty;

            var environments = results.Stories.SelectMany(s => s.Environments).Select(e => e.Name).Distinct();
            foreach (var environment in environments)
            {
                var cycle = new EnvironmentCycleProxy {name = environment};
                cycles.Add(environment, cycle);
            }

            foreach (var story in results.Stories)
            {
                storyName = story.Description;
                storyIds.Add(story.Id);

                var testRuns = from e in story.Environments
                               let acceptedDate = e.Results.Where(r => r.Accepted).Max(r => r.TestTime)
                               from r in e.Results
                               where r.Accepted && r.TestTime == acceptedDate
                               orderby r.TestRun
                               select new {Environment = e.Name, r};

                var firstRunNum = story.Environments.SelectMany(r => r.Results).Min(r => r.TestRun);
                var firstRun = story.Environments.SelectMany(r => r.Results).Single(r => r.TestRun == firstRunNum);

                var startDate = firstRun.TestTime;
                var prevDate = startDate;
                foreach (var testRun in testRuns)
                {
                    var environment = testRun.Environment;
                    var envCycle = cycles[environment];
                    var cycleTimes = envCycle.cycleTimes;

                    var testResult = testRun.r;

                    var endDate = testResult.TestTime;
                    var diff = endDate.Subtract(prevDate);

                    var cycle = new CycleDuration();
                    cycle.start = prevDate;
                    cycle.end = endDate;
                    cycle.days = diff.Days;

                    cycleTimes.Add(cycle);

                    prevDate = endDate;
                }
            }

            var environmentCycleTimes = new List<EnvironmentCycle>();
            foreach (var proxy in cycles)
            {
                var cycleTime = new EnvironmentCycle {name = proxy.Key};
                cycleTime.CycleDurations = proxy.Value.cycleTimes.ToArray();
                environmentCycleTimes.Add(cycleTime);
            }


            return new StoryCycleDuration
                       {
                           storyName = storyName,
                           stories = storyIds.ToArray(),
                           environmentCycle = environmentCycleTimes.ToArray()
                       };
        }

        public ChartData GetStoryChartData(string spec, string id)
        {
            var results = GetResultsByEnvironmentStory(spec, id);

            var data = new ChartData();
            data.storyName = GetStoryName(results);
            var startDate = GetStartDate(results);

            var seriesList = new List<Series>();
            var successHeight = 2;
            var successVal = successHeight;
            foreach (var environment in results.Environments)
            {
                var series = new Series();
                series.environment = environment.Name;

                var story = environment.Stories.FirstOrDefault();
                if (story == null)
                    continue;

                var dataPoints = new List<Point>();
                int prevValue = successVal - successHeight;

                Point pt;

                // Initial Point
                pt = new Point(startDate, prevValue, false);
                dataPoints.Add(pt);

                foreach (var result in story.Results)
                {
                    var day = result.TestTime.Date;
                    RemovePreviousDate(day, dataPoints); // Remove any prev points for the same day

                    var value = successVal - successHeight;
                    if (result.Result == "Success")
                        value = successVal;

                    if (value != prevValue) // Add dummy point to display instant change
                    {
                        pt = new Point(result.TestTime, prevValue, false);
                        dataPoints.Add(pt);
                    }

                    pt = new Point(result.TestTime, value);
                    pt.testRun = result.TestRun;
                    pt.result = result.Result;

                    if (result.Accepted)
                        pt.accepted = result.Accepted;

                    dataPoints.Add(pt);

                    prevValue = value;
                }

                successVal += successHeight + 1;
                series.points = dataPoints.ToArray();
                seriesList.Add(series);
            }


            data.seriesArray = seriesList.ToArray();

            return data;
        }

        private static void RemovePreviousDate(DateTime date, List<Point> dataPoints)
        {
            var prevTime = dataPoints.Where(p => p.x.Date == date);
            if (!prevTime.Any()) return;
            foreach (var point in prevTime.ToArray())
            {
                dataPoints.Remove(point);
            }
        }

        private string GetStoryName(Project results)
        {
            var env = results.Environments.FirstOrDefault();
            if (env == null)
                return null;

            var story = env.Stories.FirstOrDefault();
            if (story == null)
                return null;

            return story.Description;
        }

        private DateTime GetStartDate(Project results)
        {
            var testTimes = from e in results.Environments
                            from s in e.Stories
                            from r in s.Results
                            select r.TestTime;

            return testTimes.Min();
        }
    }

    public class StoryCycleDuration
    {
        public string storyName;
        public string[] stories;
        public EnvironmentCycle[] environmentCycle;
    }

    public class EnvironmentCycle
    {
        public string name;
        public CycleDuration[] CycleDurations;
    }

    public class EnvironmentCycleProxy
    {
        public string name;
        public List<CycleDuration> cycleTimes = new List<CycleDuration>();
    }

    public class CycleDuration
    {
        public int days;
        public DateTime start;
        public DateTime end;
        public bool enableAnnotation;
        public string annotation;
    }

    public class ChartData
    {
        public string storyName;
        public Series[] seriesArray;
    }

    public class Series
    {
        public string environment;
        public Point[] points;
    }

    public class Point
    {
        public Point(DateTime x, int y)
            : this(x, y, true)
        {
        }

        public Point(DateTime x, int y, bool enabled)
        {
            this.x = x;
            this.y = y;
            this.enabled = enabled;
        }
        public DateTime x;
        public int y;
        public bool enabled = true;
        public bool accepted;
        public int testRun;
        public string result;
    }
}