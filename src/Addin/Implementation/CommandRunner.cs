// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using System.Management;
using System.Runtime.InteropServices;
using Process = EnvDTE.Process;


namespace DeploymentFrameworkForBizTalk.Addin.Implementation
{
    internal class CommandRunner
    {
        internal const string ToolWindowName = "Deployment Framework for BizTalk";

        internal int IsBusy = 0;

        private DTE2 _applicationObject;
        private IVsOutputWindow _vsOutputWindow;

        private delegate void RunHandler(string exePath, string arguments);

        internal CommandRunner(DTE2 applicationObject, IVsOutputWindow outputWindow)
        {
            this._applicationObject = applicationObject;
            this._vsOutputWindow = outputWindow;
        }

        internal void ExecuteBuild(string exePath, string arguments)
        {
            if (SetBusy() == 1)
            {
                return;
            }

            OutputWindow ow = _applicationObject.ToolWindows.OutputWindow;
            OutputWindowPane owP = GetOutputWindowPane();

            owP.Clear();
            owP.Activate();
            ow.Parent.Activate();

            RunHandler rh = new RunHandler(Run);
            AsyncCallback callback = new AsyncCallback(RunCallback);
            rh.BeginInvoke(exePath, arguments, callback, rh);
        }

        internal void OnOpenSolution()
        {
            GetOutputWindowPane();
        }

        internal void OnCloseSolution()
        {
            RemoveOutputWindowPane();
        }

        private int SetBusy()
        {
            return Interlocked.CompareExchange(ref IsBusy, 1, 0);
        }

        private void SetFree()
        {
            Interlocked.CompareExchange(ref IsBusy, 0, 1);
        }

        private void RunCallback(IAsyncResult result)
        {
            RunHandler rh = result.AsyncState as RunHandler;

            try
            {
                rh.EndInvoke(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Deployment Framework for BizTalk: Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetFree();
            }
        }

        private void proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            WriteToOutputWindow(e.Data);
        }

        private void WriteToOutputWindow(string message)
        {
            OutputWindowPane owP = GetOutputWindowPane();
            owP.OutputString(message);
            owP.OutputString("\r\n");
        }

        private void Run(string exePath, string arguments)
        {
            RunProcess(exePath, arguments);
        }

        private void RunProcess(string exePath, string arguments)
        {
            WriteToOutputWindow("Starting build...");
            WriteToOutputWindow(exePath + " " + arguments);
            WriteToOutputWindow(string.Empty);

            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = exePath;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = true;

                proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(proc_OutputDataReceived);
                proc.Start();

                proc.BeginOutputReadLine();
                proc.WaitForExit();
            }
        }

        private OutputWindowPane GetOutputWindowPane()
        {
            OutputWindow ow = _applicationObject.ToolWindows.OutputWindow;

            foreach (OutputWindowPane owp in ow.OutputWindowPanes)
            {
                if (string.Compare(owp.Name, ToolWindowName, true) == 0)
                {
                    return owp;
                }
            }

            return ow.OutputWindowPanes.Add(ToolWindowName);
        }

        private void RemoveOutputWindowPane()
        {
            OutputWindow ow = _applicationObject.ToolWindows.OutputWindow;

            foreach (OutputWindowPane owp in ow.OutputWindowPanes)
            {
                if (string.Compare(owp.Name, ToolWindowName, true) == 0)
                {
                    _vsOutputWindow.DeletePane(Guid.Parse(owp.Guid));
                    return;
                }
            }
        }

        public void Attach(ManagementObject biztalkHostObject, bool bounceHost)
        //{
        //                var bizTalkHostName = managementObject.Properties["HostName"].Value.ToString();

        //    Attach(processName, bizTalkHostName, bounceHost);
        //}
        //public void Attach(string processName, string bizTalkHostName, bool bounceHost = false)
        {
            // Try loop - Visual Studio may not respond the first time.
            var tryCount = 5;
            while (tryCount-- > 0)
            {
                try
                {
                    if (bounceHost)
                    {
                        //owP.OutputString(Environment.NewLine);

                        var i = biztalkHostObject; //GetBiztalkHosts().SingleOrDefault(h => h.Key == bizTalkHostName).Value;

                        if (i != null)
                        {
                            WriteToOutputWindow("Stopping: " + i.Properties["HostName"].Value);
                            i.InvokeMethod("Stop", null);

                            WriteToOutputWindow("Starting: " + i.Properties["HostName"].Value);
                            i.InvokeMethod("Start", null);
                        }
                    }


                    var processName = biztalkHostObject.Properties["HostName"].Value.ToString();
                    var perfCounter = new System.Diagnostics.PerformanceCounter("BizTalk:Messaging", "ID Process", processName);
                    var processID = perfCounter.NextValue();

                    var processes = _applicationObject.Debugger.LocalProcesses;
                    foreach (var proc in from Process proc in processes /*where proc.Name.IndexOf(processName, StringComparison.OrdinalIgnoreCase) != -1*/ where proc.ProcessID == processID let serviceName = GetServiceName(proc.ProcessID) /*where String.Equals(serviceName, string.Format("btssvc${0}", bizTalkHostName), StringComparison.OrdinalIgnoreCase)*/ select proc)
                    {
                        proc.Attach();
                        
                        WriteToOutputWindow(Environment.NewLine);
                        WriteToOutputWindow(String.Format("Attached to process {0} - {1} successfully.", processName, proc.ProcessID));
                        WriteToOutputWindow(Environment.NewLine);
                        WriteToOutputWindow(Environment.NewLine);

                        break;
                    }
                    break;
                }
                catch (COMException)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }


        public static String GetServiceName(int processId)
        {
            var query = "SELECT * FROM Win32_Service where ProcessId = " + processId;
            var searcher = new ManagementObjectSearcher(query);

            var retVal = (from ManagementObject queryObj in searcher.Get() select queryObj["Name"].ToString()).FirstOrDefault();

            return retVal;

        }
    }
}
