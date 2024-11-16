using Armoire.Models;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels
{
 
    public partial class ApplicationMonitorViewModel: ViewModelBase
    {
        public static ObservableCollection<ItemViewModel> ProcessList { get; } = new ObservableCollection<ItemViewModel>();

        public static ObservableCollection<Process> RunningApps { get; set; } = new ObservableCollection<Process>();
        public static ObservableCollection<Item> runningApplications { get; set; } = [];

        public static Dictionary<int, string> processMap = new Dictionary<int, string>();

        public static ObservableCollection<int> Pids =  new ObservableCollection<int>();

      

        private static bool isMonitoring = true;

        public  static bool isRunning;

        public ApplicationMonitorViewModel() { 
        
        }



        [RelayCommand]
        public static void DisplayProcess()
        {
            for (int j = 0; j < RunningApps.Count; j++)
            {

                if (ProcessList.Count > 0)
                {
                    for (int i = 0; i < ProcessList.Count; i++)
                    {
                        bool isNewProcess = ProcessList.Any(obj => obj.ExecutablePath == RunningApps[j].StartInfo.FileName);
                        if (!isNewProcess)
                        {
                            ProcessList.Add(new ItemViewModel(RunningApps[j].StartInfo.FileName));
                            return;
                        }
                    }


                }
                else
                {
                    ProcessList.Add(new ItemViewModel(RunningApps[j].StartInfo.FileName));
                }

            }
        }


        public static async Task CheckRunningApplication()
        {

        
            while (isMonitoring)
            {
                await Task.Delay(500);
                var processes = Process.GetProcesses();

                

                List<Process> ProcessWithName = new List<Process>();

                foreach (Process process in processes)
                {
                    if (!String.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        ProcessWithName.Add(process);

                    }
                }



                
                
                

            

            }

        }



        /**
        public static async Task CheckRunningApplication()
        {
            RunningApps = new ObservableCollection<string>();
            Process[] updatedProcessList;
            Process[] initialProcesses = Process.GetProcesses();
            foreach (Process process in initialProcesses)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    RunningApps.Add(process.MainWindowTitle);
                }
            }
            while (isMonitoring) { 
                //isRunning = true;
                await Task.Delay(1000);
                updatedProcessList = Process.GetProcesses();
                ArrayList updatedProcessNames = new ArrayList();
                foreach (Process p in updatedProcessList) 
                {
                    if (!String.IsNullOrEmpty(p.MainWindowTitle))
                    {
                        updatedProcessNames.Add(p.MainWindowTitle);
                    }
                }
                foreach (string processName in RunningApps)
                {
                    if(!updatedProcessNames.Contains(processName))
                        RunningApps.Remove(processName);
                }
                foreach(string processName in updatedProcessNames)
                {
                    if(!RunningApps.Contains(processName))
                        RunningApps.Add(processName);
                }
                //for(int i = 0; i < Pids.Count; i++) {

                //    if (processMap.ContainsKey(Pids[i])) {

                //        var key = Pids[i];

                //        if (Process.GetProcessById(key).HasExited)
                //        {
                //            isRunning = false;
                //            if (runningApplications[i].ExecutablePath == processMap[Pids[i]])
                //            {
                //                runningApplications.RemoveAt(i);
                //                processMap.Remove(Pids[i]);
                //                Pids.RemoveAt(i);
                //            }

                //        } else { 
                //            isRunning = true; 
                //        }        

                //    }
                //}

               
            }

            isRunning = false;
        }
        */



    }
}
