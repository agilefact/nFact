using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AcceptanceTests.Scenarios;
using OpenQA.Selenium;

namespace AcceptanceTests.ScreenCapture
{
    public class ScreenshotCapture
    {
        private readonly string _artifactsDir;
        private readonly ITakesScreenshot _capture;
        private static int _imageCount;
        public ScreenshotCapture(IWebDriver driver, string artifactsDir)
        {
            _artifactsDir = artifactsDir;
            _capture = driver as ITakesScreenshot;
            if (_capture == null)
                throw new ApplicationException("Web Driver is not of interfact ITakesScreenshot");
        }

        public ScreenshotCapture(IWebDriver driver)
        {
            _artifactsDir = Path.Combine(Environment.CurrentDirectory, "artifacts");
            if (!Directory.Exists(_artifactsDir))
                Directory.CreateDirectory(_artifactsDir);

            _capture = driver as ITakesScreenshot;
            if (_capture == null)
                throw new ApplicationException("Web Driver is not of interfact ITakesScreenshot");
        }

        public void Take()
        {
            _imageCount++;
            var scenarioCount = ScanarioManager.ScenarioCount.ToString(CultureInfo.InvariantCulture);
            var fileName = string.Format("{0}.png", _imageCount);
            var path = Path.Combine(_artifactsDir, scenarioCount);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filePath = Path.Combine(path, fileName);
            var ss =_capture.GetScreenshot();
            ss.SaveAsFile(filePath, ImageFormat.Png);
        }
    }
}
