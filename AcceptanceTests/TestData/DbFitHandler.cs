using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Xml.XPath;
using AcceptanceTests.Configuration;

namespace AcceptanceTests.TestData
{
    public class DbFitHandler
    {
        private readonly string _domain;

        public DbFitHandler()
            : this(TestConfigurationManager.Settings["DBFit.Domain"])
        {}

        public DbFitHandler(string domain)
        {
            _domain = domain;
        }

        public void Setup(string script)
        {
            GetScriptResult(script, "setup");
        }

        public void Teardown(string script)
        {
            GetScriptResult(script, "teardown");
        }

        public void GetScriptResult(string script, string tag)
        {
            try
            {
                DbFitRequest(script);
            }
            finally
            {
                var resultsUrl = GetResultsUrl(script);
                if (resultsUrl != null)
                {
                    Console.Write("@{0}", tag);
                    Console.Write("{");
                    Console.Write(resultsUrl);
                    Console.WriteLine("}");
                }

            }
        }

        public string GetResultsUrl(string script)
        {
            var xelement = GetRequestXml(script, "pageHistory&format=xml");
            var testResult = ((IEnumerable)xelement.XPathEvaluate("/Page/TestResult")).Cast<XElement>().FirstOrDefault();
            
            if (testResult == null || testResult.Element("ResultLink") == null)
                return null;

            var resultsUrl = testResult.Element("ResultLink").Value;
            resultsUrl = resultsUrl.TrimEnd("&format=xml".ToCharArray());

            return Path.Combine(_domain, resultsUrl);
        }

        private void DbFitRequest(string url)
        {
            var xelement = GetRequestXml(url, "test&format=xml");
            var testResult = xelement.Descendants("counts").Single();
            var right = Parse(testResult, "right");
            var wrong = Parse(testResult, "wrong");
            var ignores = Parse(testResult, "ignores");
            var exceptions = Parse(testResult, "exceptions");

            if (wrong > 0)
                throw new ApplicationException(string.Format("There were {0} tests wrong for DBFit url {1}", wrong, url));

            if (exceptions > 0)
                throw new ApplicationException(string.Format("There were {0} exceptions for DBFit url {1}", exceptions, url));
        }

        private XElement GetRequestXml(string relUrl, string queryParams)
        {
            var url = Path.Combine(_domain, relUrl);
            url = string.Format("{0}?{1}", url, queryParams);
            var stream = GetRequest(url);
            var xelement = XElement.Load(stream);
            return xelement;
        }

        private static Stream GetRequest(string url)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            var response = request.GetResponse() as HttpWebResponse;
            if (response.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException(string.Format("Status code: {0} returned for request {1}", response.StatusCode,
                                                             url));

            var stream = response.GetResponseStream();
            return stream;
        }

        private static int Parse(XElement xml, string label)
        {
            return int.Parse(xml.Element(label).Value);
        }
    }
}