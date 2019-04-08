using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Doroshenko05
{
    internal class ManagerProcessViewModel : INotifyPropertyChanged
    {
        private readonly Action<bool> _showLoaderAction;
        private ObservableCollection<SystemProcess> _processes;
        private readonly Thread _threadUpdate;
        private SystemProcess _checkedProc;


        public bool IsItemSelected => ProcSelect != null;

        public SystemProcess ProcSelect
        {
            get => _checkedProc;
            set
            {
                _checkedProc = value;
                OnPropertyChanged();
                OnPropertyChanged("IsItemSelected");
            }
        }

        private async void ProcInit()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate { _showLoaderAction.Invoke(true); });
            await Task.Run(() =>
            {
                Processes = new ObservableCollection<SystemProcess>(SystemProcessThreadsManipulation.Processes.Values);
            });
            _threadUpdate.Start();
            while (SystemProcessThreadsManipulation.Processes.Count == 0)
                Thread.Sleep(3000);
            System.Windows.Application.Current.Dispatcher.Invoke(delegate { _showLoaderAction.Invoke(false); });
        }

        internal void Close()
        {
            _threadUpdate.Join(3000);
        }

        public ObservableCollection<SystemProcess> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }


        internal ManagerProcessViewModel(Action<bool> showLoaderAction)
        {
            _showLoaderAction = showLoaderAction;
            _threadUpdate = new Thread(ProcUpdate);
            Thread initializationThread = new Thread(ProcInit);
            initializationThread.Start();
        }


        private RelayCommand _finishProcessCommand;
        private RelayCommand locationCommand;
        public RelayCommand FinishProcessCommand => _finishProcessCommand ?? (_finishProcessCommand = new RelayCommand(FinishProc));
        public RelayCommand LocationCommand => locationCommand ?? (locationCommand = new RelayCommand(LocationProc));
        private void FinishProc(object o)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(ProcSelect.ProcessId);
                try
                {
                    process.Kill();
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private async void LocationProc(object o)
        {
            await Task.Run(() =>
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(ProcSelect.ProcessId);
                try
                {
                    string fullPath = process.MainModule.FileName;
                    System.Diagnostics.Process.Start("explorer.exe", fullPath.Remove(fullPath.LastIndexOf('\\')));
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private async void ProcUpdate()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(delegate
                    {
                        try
                        {
                            lock (Processes)
                            {
                                List<SystemProcess> procList = new List<SystemProcess>(SystemProcessThreadsManipulation.Processes.Values.Where(proc => !Processes.Contains(proc)));
                                foreach (SystemProcess proc in procList)
                                {
                                    Processes.Add(proc);
                                }
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (ArgumentNullException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (InvalidOperationException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    });
                });
                Thread.Sleep(4000);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
