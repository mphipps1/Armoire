using Armoire.Models;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels
{
 
    public partial class ApplicationMonitorViewModel: DrawerAsContentsViewModel
    {
        public static ObservableCollection<ItemViewModel> ProcessList { get; } = new ObservableCollection<ItemViewModel>();

        public static List<string> RunningAppNames { get; set; } = new List<string>();
      

        private static bool isMonitoring = true;

        public  static bool isRunning;

        public ApplicationMonitorViewModel(string? parentID, int drawerHierarchy) : base (parentID, drawerHierarchy) {
            Name = "Running Applications";

        }



        //[RelayCommand]
        //public void DisplayProcess()
        //{
        //    for (int j = 0; j < 5; j++)
        //    {

        //        if (ProcessList.Count > 0)
        //        {
        //            for (int i = 0; i < ProcessList.Count; i++)
        //            {
        //                bool isNewProcess = ProcessList.Any(obj => obj.ExecutablePath == RunningApps[j].StartInfo.FileName);
        //                if (!isNewProcess)
        //                {
        //                    ProcessList.Add(new ItemViewModel(RunningApps[j].StartInfo.FileName));
        //                    return;
        //                }
        //            }


        //        }
        //        else
        //        {
        //            ProcessList.Add(new ItemViewModel(RunningApps[j].StartInfo.FileName));
        //        }

        //    }
        //}

        public async void GetInitialRunningApps()
        {
            ArrayList badProcesses = new ArrayList();
            badProcesses.Add("TextInputHost");
            badProcesses.Add("devenv");
            var processes = Process.GetProcesses();
            var checkingApps = CheckRunningApplication(this);

            List<Process> ProcessWithName = new List<Process>();

            foreach (Process process in processes)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    if (badProcesses.Contains(process.ProcessName))
                        continue;
                    ProcessWithName.Add(process);
                    RunningAppNames.Add(process.ProcessName);
                    GeneratedDrawer.Contents.Add(new RunningItemViewModel(Id, DrawerHierarchy, GeneratedDrawer, process));
                }
            }

            await checkingApps;

            
        }

        //Work in Progress
        public static async Task CheckRunningApplication(DrawerAsContentsViewModel dac)
        {

        
            while (isMonitoring)
            {
                await Task.Delay(1000);
                var processes = Process.GetProcesses();

                
                Dictionary<string, Process> apps = new Dictionary<string, Process>();

                foreach (Process process in processes)
                {
                    if (!String.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        apps.Add(process.ProcessName, process);
                    }
                }

                foreach (string appName in RunningAppNames.ToList())
                {
                    if (!apps.Keys.Contains(appName))
                    {
                        RunningAppNames.Remove(appName);
                        foreach(var cuvm in dac.GeneratedDrawer.Contents.ToList())
                        {
                            if (cuvm.Name.Equals(appName))
                                cuvm.DeleteMe = true;
                        }
                    }
                }

                foreach (string appName in apps.Keys)
                {
                    if (!RunningAppNames.Contains(appName))
                    {
                        RunningAppNames.Add(appName);
                        dac.GeneratedDrawer.Contents.Add(new RunningItemViewModel(dac.Id, dac.DrawerHierarchy, dac.GeneratedDrawer, apps[appName]));
                    }

                }
                
                //for (int i = 0; i < RunningApps.Count; i++)
                //{
                //    var GetProcess = Process.GetProcesses()[i];

                    
                //   if(GetProcess == null)
                //    {
                //        var itemviewmodel = ProcessList.FirstOrDefault(obj => obj.ExecutablePath == RunningApps[i].StartInfo.FileName);
                //        ProcessList.Remove(itemviewmodel);
                //        RunningApps.RemoveAt(i);

                //    } 
                    
                //}
                

            

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
