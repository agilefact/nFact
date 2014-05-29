using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Owin.Hosting;
using nFact.Shared;

namespace nFact
{
    internal class Program
    {
        static string url = "http://localhost:9999";

        private const string OneInstanceMutexName = @"Global\nFact";

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            if (args.Contains("-url"))
                url = GetUrl(args);

            // Mutex only permits one instance of the application to be running.
            using (var mutex = new Mutex(false, OneInstanceMutexName))
            {
                // Wait a few seconds if contended, in case another instance
                // of the program is still in the process of shutting down.

                try
                {
                    if (!mutex.WaitOne(TimeSpan.FromSeconds(7), false))
                    {
                        Console.WriteLine(string.Format("There is another instance running on url: {0}", url));
                        return;
                    }
                }
                catch(AbandonedMutexException )
                {
                    Console.WriteLine("Another instance on same url has just terminated.");
                }

                using (WebApplication.Start<Startup>(url))
                {
                    Console.WriteLine("Server running on {0}", url);
                    Console.WriteLine("Press <ENTER> to terminate");
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = new ScriptLogger();
            logger.LogError(e.ExceptionObject.ToString());
            ProcessController.TryKillProcess("nunit-agent");
        }

        private static string GetUrl(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-url" && args.Length > i + 1)
                    return args[i + 1];
            }
            return url;
        }

        public static void Restart()
        {
            ProcessController.TryKillProcess("nunit-agent");
            var args = string.Format("-url {0}", url);
            Process.Start(Application.ExecutablePath, args);

            Thread.Sleep(2000);

            Environment.Exit(-1);
        }
    }
}
