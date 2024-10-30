using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Armoire.Interfaces;

namespace Armoire.Models;

public class Item : ContentsUnit
{
    // [Key] -> `DrawerId` is the primary key.
    // [Database...] -> A new `DrawerId` gets generated for each row inserted.
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ItemId { get; set; }

    [MaxLength(100)]
    public string ExecutablePath { get; set; } = "default";

    private Process process { get; set; }

    public void Execute()
    {
        process.Start();
    }

    // Parameterless constructor for EF.
    public Item() { }

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
}
