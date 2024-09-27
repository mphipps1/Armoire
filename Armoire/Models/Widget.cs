namespace Armoire.Models;

public class Widget : IContentsUnit
{
    public string Name { get; set; }
    public string? IconPath { get; set; }

    public Widget(string name, string? iconPath)
    {
        Name = name;
        IconPath = iconPath;
    }
}
