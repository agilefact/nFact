using System;
using System.Diagnostics;

namespace nFact.Shared
{
    public class ProcessController
    {
        public static Process InvokeProcess(string processName, string parameters)
        {
            var startInfo = new ProcessStartInfo(processName, parameters);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = true;
            startInfo.ErrorDialog = false;

            var proc = new Process();
            proc.StartInfo = startInfo;
            proc.Start();

            return proc;
        }

        public static void TryKillProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process proc in processes)
            {
                try { proc.Kill(); }
                catch (Exception exp) { }
            }
        }
    }
}