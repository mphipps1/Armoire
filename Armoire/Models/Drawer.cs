using System.Collections.Generic;
using Armoire.Interfaces;

namespace Armoire.Models;

public class Drawer : IDrawer
{
    public int Position { get; set; }

    private string? _name;

    public string Name
    {
        get => _name ?? $"default{DrawerId}";
        set => _name = value;
    }

    public string IconPath { get; set; } = "default";

    public IDrawer? ParentDrawer { get; set; }

    public long DrawerId { get; set; }
    public List<IDrawer> Drawers { get; set; } = [];
    public List<IItem> Items { get; set; } = [];
    public long ParentDrawerId { get; set; }
}
