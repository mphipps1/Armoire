using Armoire.Interfaces;

namespace Armoire.Models;

public class Widget 
{
    public string Name { get; set; }
    public string IconPath { get; set; }
    public Drawer? ParentDrawer { get; set; }
    public long ParentDrawerId { get; set; }

    public Widget(string name, string? iconPath)
    {
        Name = name;
        IconPath = iconPath;
    }
}
