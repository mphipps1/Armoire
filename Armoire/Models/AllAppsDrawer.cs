using Armoire.Interfaces;

namespace Armoire.Models;

public class AllAppsDrawer : IContentsUnit, IDrawer
{
    public string Name { get; set; }
    public string? IconPath { get; set; }

    public AllAppsDrawer(string name)
    {
        Name = name;
    }
}
