using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;

namespace Doroshenko05
{
    internal class SystemProcess
    {
        public string ProcessName
        {
            get;
            set;
        }

        public int ProcessId
        {
            get;
            set;
        }

        public bool ProcessActive
        {
            get;
            set;
        }

        public int ProcessCpu
        {
            get;
            set;
        }

        public int ProcessRam
        {
            get;
            set;
        }

        public int ProcessThreadsCount
        {
            get;
            set;
        }

        public string ProcessOwner
        {
            get;
            set;
        }

        public string ProcessLocationPath
        {
            get;
            set;
        }

        public string ProcessStartTime
        {
            get;
            set;
        }

        internal PerformanceCounter RamCounter { get; }

        internal PerformanceCounter CpuCounter { get; }

        internal SystemProcess(Process systemProcess)
        {
            //INTERNET SAMPLE
            RamCounter = new PerformanceCounter("Process", "Working Set", systemProcess.ProcessName);
            CpuCounter = new PerformanceCounter("Process", "% Processor Time", systemProcess.ProcessName);
            ProcessName = systemProcess.ProcessName;
            ProcessId = systemProcess.Id;
            ProcessActive = systemProcess.Responding;
            ProcessCpu = (int)CpuCounter.NextValue();
            ProcessRam = (int)(RamCounter.NextValue() / 1024 / 1024);
            ProcessThreadsCount = systemProcess.Threads.Count;
            ProcessOwner = ProcOwner(systemProcess.Id);
            try
            {
                ProcessLocationPath = systemProcess.MainModule.FileName;
            }
            catch (Win32Exception e)
            {
                ProcessLocationPath = e.Message;
            }
            try
            {
                ProcessStartTime = systemProcess.StartTime.ToString();
            }
            catch (Win32Exception e)
            {
                ProcessStartTime = e.Message;
            }
        }

        private static string ProcOwner(int processId)
        {
            string search = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(search);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] procList = { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", procList));
                if (returnVal == 0)
                {                    
                    return procList[1] + "\\" + procList[0];
                }
            }
            return "SYSTEM";
        }
    }
}
