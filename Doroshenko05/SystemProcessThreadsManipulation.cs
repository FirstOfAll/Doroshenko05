using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Threading;

namespace Doroshenko05
{
    internal static class SystemProcessThreadsManipulation
    {
        private static readonly Thread DataManipulationUpdate;
        private static readonly Thread DataThreadUpdate;

        internal static Dictionary<int, SystemProcess> Processes { get; set; }

        static SystemProcessThreadsManipulation()
        {
            Processes = new Dictionary<int, SystemProcess>();
            DataThreadUpdate = new Thread(ThreadUpdate);
            DataManipulationUpdate = new Thread(ThreadManipulation);
            DataManipulationUpdate.Start();
            DataThreadUpdate.Start();
        }

        private static async void ThreadUpdate()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    lock (Processes)
                    {
                        foreach (int procId in Processes.Keys.ToList())
                        {
                            System.Diagnostics.Process a;
                            try
                            {
                                a = System.Diagnostics.Process.GetProcessById(procId);
                            }
                            catch (ArgumentException)
                            {
                                Processes.Remove(procId);
                                continue;
                            }

                            Processes[procId].ProcessCpu = (int)Processes[procId].CpuCounter.NextValue();
                            Processes[procId].ProcessRam = (int)(Processes[procId].RamCounter.NextValue() / 1024 / 1024);
                            Processes[procId].ProcessThreadsCount = a.Threads.Count;
                        }
                    }
                });
                Thread.Sleep(2000);
            }
        }

        internal static void Close()
        {
            DataManipulationUpdate.Join(4000);
            DataThreadUpdate.Join(1500);
        }

        private static async void ThreadManipulation()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    lock (Processes)
                    {
                        List<System.Diagnostics.Process> processes = System.Diagnostics.Process.GetProcesses().ToList();
                        IEnumerable<int> keys = Processes.Keys.ToList()
                            .Where(id => processes.All(proc => proc.Id != id));
                        foreach (int key in keys)
                        {
                            Processes.Remove(key);
                        }

                        foreach (System.Diagnostics.Process proc in processes)
                        {
                            if (!Processes.ContainsKey(proc.Id))
                            {
                                try
                                {
                                    Processes[proc.Id] = new SystemProcess(proc);
                                }
                                catch (InvalidOperationException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch (ManagementException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch (NullReferenceException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                });
                Thread.Sleep(5000);
            }
        }

        
    }
}
