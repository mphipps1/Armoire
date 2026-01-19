/*  This class is currently incomplete, but contains our attempt at creating jumplists for
 *  Items in Armoire.
 * 
 */

using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Armoire.ViewModels
{
    public static class JumpListViewModel
    {
        private static List<FileInfo> fileInfos = new List<FileInfo>();

        public static List<ItemViewModel> Items = new List<ItemViewModel>();

        public static List<string> lnkpaths = new List<string>();   


        public static void GetJumpList()
        {
            string userName = Environment.UserName;
            string directloc = $"C:\\Users\\{userName}\\AppData\\Roaming\\Microsoft\\Windows\\Recent";

            // files list from the root directory and prints it
            string[] fyles = Directory.GetFiles(directloc);
            string file;
            for (int i = 0; i < fyles.Length; i++)
            {

                fileInfos.Add(new FileInfo(fyles[i]));
                Type shellObjectType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic windowsShell = Activator.CreateInstance(shellObjectType);
                if (fileInfos.ElementAt(i).Extension.Equals(".lnk"))
                {
                    dynamic shortcut = windowsShell.CreateShortcut(fileInfos.ElementAt(i).FullName);
                    lnkpaths.Add(fyles[i]);
                }

                //   var file = shortcut.TargetPath;
                string applicationName = Path.GetFileNameWithoutExtension(fileInfos.ElementAt(i).FullName);
            }

         }




        public static string GetOpenWithAppName()
        {

            string autoDestPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                            @"Microsoft\Windows\Recent\AutomaticDestinations");

            foreach (var file in Directory.GetFiles(autoDestPath, "*.automaticDestinations-ms"))
            {
                Console.WriteLine($"Jump List File: {Path.GetFileName(file)}");
            }


            return "";
        }
               
    }

     


    }




