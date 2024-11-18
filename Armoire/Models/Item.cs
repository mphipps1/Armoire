using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using Armoire.ViewModels;

namespace Armoire.Models;

public class Item
{
    // [Key] -> `Id` is the primary key.
    [Key]
    [MaxLength(100)]
    public string Id { get; set; } = "default";

    [MaxLength(100)]
    public string Name { get; set; } = "default";

    [MaxLength(100)]
    public string ExecutablePath { get; set; } = "default";

    private Process ActiveProcess { get; }

    public int? Position { get; set; }

    public void Execute()
    {
        ActiveProcess.Start();
        //ApplicationMonitorViewModel.RunningApps.Add(process);
        //ApplicationMonitorViewModel.DisplayProcess();
        var b = MainWindowViewModel.TaskCheck?.Status;
    }

    // Parameterless constructor needed so EF can build the schema.
    public Item() { }

    public virtual Drawer Parent { get; set; }

    public int? DrawerHierarchy { get; set; }

    [MaxLength(100)]
    public string ParentId { get; set; } = "default";

    public Item(Item item)
    {
        Id = item.Id;
        Name = item.Name;
        ExecutablePath = item.ExecutablePath;
        Position = item.Position;
        DrawerHierarchy = item.DrawerHierarchy;
        ParentId = item.ParentId;
        ActiveProcess = new Process();
        ActiveProcess.StartInfo.FileName = item.ExecutablePath;
        ActiveProcess.StartInfo.UseShellExecute = true;
        Parent = new Drawer(item.Parent);
    }

    public Item(
        string id,
        string name,
        string exePath,
        string parentId,
        int? position,
        int? drawerHierarchy
    )
        : this(name, exePath, parentId, position)
    {
        Id = id;
        DrawerHierarchy = drawerHierarchy;
    }

    public Item(string name, string path, string parentId, int? position = null)
    {
        Name = name;
        //make a new process out of the path
        //the old method gave an error when accessing other folders
        //this method also doesnt require proper quoting around folders or the executable name if it has a space in it
        //System.Diagnostics.Process.Start(path);
        ActiveProcess = new Process();
        ActiveProcess.StartInfo.FileName = path;
        ActiveProcess.StartInfo.UseShellExecute = true;
        ExecutablePath = path;
        ParentId = parentId;
        Position = position;
    }
}
