
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

            var local = results.Environments.First();
            var series = new List<Point>();
            foreach (var story in local.Stories)
            {
                foreach (var result in story.Results)
                {
                    var value = 0;
                    if (result.Result == "Success")
                        value = 1;
                    var pt = new Point(result.TestTime, value);
                    series.Add(pt);
                }
            }
            var data = new ChartData();

            data.series = new Series();
            data.series.points = series.ToArray();

            return data;
        }
    }

    public class ChartData
    {
        public Series series;
    }

    public class Series
    {
        public Point[] points;
    }

    public class Point 
    {
        public Point(DateTime x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public DateTime x;
        public int y;
    }
}
