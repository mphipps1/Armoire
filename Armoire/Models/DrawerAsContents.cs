namespace Armoire.Models;

public class DrawerAsContents : IContentsUnit
{
    public string Name { get; set; }
    public string? IconPath { get; set; }

    public DrawerAsContents(string name)
    {
        Name = name;
    }
}
