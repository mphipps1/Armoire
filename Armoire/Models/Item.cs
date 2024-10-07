using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Armoire.Interfaces;
using Material.Dialog.ViewModels;

namespace Armoire.Models
{
    public class Item : IContentsUnit
    {
        private Process process {  get; set; }
        public string ParentDrawer { get; set; }
        public string Name { get; set; }
        public string? IconPath { get; set; }

        public void Execute()
        {
            process.Start();
        }

        public Item(string name, string path, string parentDrawer)
        {
            Name = name;
            //make a new process out of the path
            //the old method gave an error when accessing other folders
            //this method also doesnt require proper quoting around folders or the executable name if it has a space in it
            //System.Diagnostics.Process.Start(path);
            process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.UseShellExecute = true;
            ParentDrawer = parentDrawer;
        }

    }
}
