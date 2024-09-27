using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.Models
{
    public class Item : IContentsUnit
    {
        public string Path { get; set; }

        public void Execute()
        {
            System.Diagnostics.Process.Start(Path);
        }

        public Item(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; set; }
        public string? IconPath { get; set; }
    }
}
