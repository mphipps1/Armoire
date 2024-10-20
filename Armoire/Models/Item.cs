using System.Diagnostics;
using Armoire.Interfaces;

namespace Armoire.Models;

public class Item : IItem
{
    private Process process { get; set; }

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
    }

    public long ItemId { get; set; }
    public string ExecutablePath { get; set; } = "default";
    public string Name { get; set; }
    public string IconPath { get; set; } = "default";
    public IDrawer? ParentDrawer { get; set; }
    public long ParentDrawerId { get; set; }
}
