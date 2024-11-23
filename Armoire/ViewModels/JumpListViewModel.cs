using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels
{
    public class JumpListViewModel
    {
        private List<FileInfo> fileInfos = new List<FileInfo>();

        public static List<ItemViewModel> Items = new List<ItemViewModel>();

        public JumpListViewModel()
        {



        }




        public void GetJumpList()
        {
            string userName = Environment.UserName;
            string directloc = $"C:\\Users\\{userName}\\AppData\\Roaming\\Microsoft\\Windows\\Recent";

            // files list from the root directory and prints it
            string[] fyles = Directory.GetFiles(directloc);

            for (int i = 0; i < fyles.Length; i++)
            {

                fileInfos.Add(new FileInfo(fyles[i]));
                Type shellObjectType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic windowsShell = Activator.CreateInstance(shellObjectType);
                // dynamic shortcut = windowsShell.CreateShortcut(fileInfos.ElementAt(i).FullName);

                //   var file = shortcut.TargetPath;
                string applicationName = Path.GetFileNameWithoutExtension(fileInfos.ElementAt(i).FullName);
            }


        }


    }
}
