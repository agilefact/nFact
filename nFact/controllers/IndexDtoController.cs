
using System;
using System.Collections.Generic;
using System.Linq;
using nFact.Engine;
using nFact.Engine.Model.DataTransfer;

namespace nFact.controllers
{
    public class IndexDtoController : IndexController
    {
        private readonly StoryManager _manager = new StoryManager();

        public Project GetCurrentProjectStoryResults(string spec, string storyId)
        {
            var project = GetProject(spec);
            if (string.IsNullOrEmpty(storyId))
                return _manager.GetCurrentProjectStoryResults(project);

            return _manager.GetCurrentProjectStoryResults(project, storyId);
        }

        public Project GetProjectStoryResults(string spec, string storyId)
        {
            var project = GetProject(spec);
            return _manager.GetProjectStoryResults(project, storyId);
        }

        public Project GetProjectStoryResults(string spec)
        {
            var project = GetProject(spec);
            return _manager.GetProjectStoryResults(project);
        }

        public bool AcceptStory(string spec, string storyId, int testRun)
        {
            var project = GetProject(spec);
            return _manager.AcceptStoryResult(project, storyId, testRun);
        }

        public bool CanAcceptStory(Engine.Model.Project project, string storyId, int testRun)
        {
            return _manager.CanAcceptStory(project, storyId, testRun);
        }

        public ChartData GetStoryChartData(string spec, string id)
        {
            var results = GetProjectStoryResults(spec, id);

            
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
        public Point(DateTime x, int y) : this(x, y, true)
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
    }
}
