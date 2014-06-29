using System;
using System.Collections.Generic;
using System.Linq;
using nFact.Engine;
using nFact.Engine.Model.DataTransfer;

namespace nFact.controllers
{
    public class ChartController : IndexDtoController
    {
        public StoryCycleTime GetStoryCycleTime(string spec, string id)
        {
            var results = GetResultsByStory(spec, id);
            var storyIds = new List<string>();
            var cycles = new List<CycleTime>();

            var storyName = string.Empty;
            foreach (var story in results.Stories)
            {
                storyName = story.Description;
                storyIds.Add(story.Id);

                var testRuns = from e in story.Environments
                               let acceptedDate = e.Results.Where(r=> r.Accepted).Max(r => r.TestTime)
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
                    var testResult = testRun.r;

                    var endDate = testResult.TestTime;
                    var diff = endDate.Subtract(prevDate);

                    var cycle = new CycleTime {name = environment};
                    cycle.start = prevDate;
                    cycle.end = endDate;
                    cycle.days = diff.Days;
                    cycles.Add(cycle);

                    prevDate = endDate;
                }

            }

            return new StoryCycleTime
                       {
                           storyName = storyName,
                           stories = storyIds.ToArray(),
                           environmentCycleTime = cycles.ToArray()
                       };
        }

        public ChartData GetStoryChartData(string spec, string id)
        {
            var results = GetResultsByEnvironment(spec, id);

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

    public class StoryCycleTime
    {
        public string storyName;
        public string[] stories;
        public CycleTime[] environmentCycleTime;
    }

    public class CycleTime
    {
        public string name;
        public int days;
        public DateTime start;
        public DateTime end;
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
    }
}