using Armoire.Models;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels
{
 
    public class ApplicationMonitorViewModel: ViewModelBase
    {

        public static ObservableCollection<Item> runningApplications { get; set; } = [];

        public static Dictionary<int, string> processMap = new Dictionary<int, string>();

        public static ObservableCollection<int> Pids =  new ObservableCollection<int>();

        private static bool isMonitoring = true;

        public  static bool isRunning;

        public ApplicationMonitorViewModel() { 
        
        }   
        


        public static async Task CheckRunningApplication()
        {
            while(isMonitoring) { 
                isRunning = true;
            
                for(int i = 0; i < Pids.Count; i++) {

                    if (processMap.ContainsKey(Pids[i])) {

                        var key = Pids[i];

                        if (Process.GetProcessById(key).HasExited)
                        {
                            isRunning = false;
                            if (runningApplications[i].ExecutablePath == processMap[Pids[i]])
                            {
                                runningApplications.RemoveAt(i);
                                processMap.Remove(Pids[i]);
                                Pids.RemoveAt(i);
                            }
                          
                        } else { 
                            isRunning = true; 
                        }        
                    
                    }
                }

                await Task.Delay(1000);
            }

            isRunning = false;
        }


       
    }
}
