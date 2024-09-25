using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.Models
{
    public class Item
    {
        public string path { get; set; }

        public void Execute()
        {
            System.Diagnostics.Process.Start(path);
        }
    }
}
