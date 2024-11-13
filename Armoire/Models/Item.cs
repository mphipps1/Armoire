using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Drawing;
using Armoire.Interfaces;

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

    private Process process { get; set; }

    private Icon? AppIcon { get; set; }

    public void Execute()
    {
        process.Start();
    }

    // Parameterless constructor needed so EF can build the schema.
    public Item() { }

    public Drawer Parent { get; set; }

    [MaxLength(100)]
    public string ParentId { get; set; } = "default";

    public Item(string id, string name, string exePath, string parentId)
        : this(name, exePath, parentId)
    {
        Id = id;
    }

    public Item(string name, string path, string parentId)
    {
        Name = name;
        //make a new process out of the path
        //the old method gave an error when accessing other folders
        //this method also doesnt require proper quoting around folders or the executable name if it has a space in it
        //System.Diagnostics.Process.Start(path);
        process = new Process();
        process.StartInfo.FileName = path;
        process.StartInfo.UseShellExecute = true;
        ExecutablePath = path;
        ParentId = parentId;
    }
}
