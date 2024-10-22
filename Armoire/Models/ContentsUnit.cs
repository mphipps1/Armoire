using System.ComponentModel.DataAnnotations;

namespace Armoire.Models;

public class ContentsUnit
{
    public int Position { get; set; }

    private string? _name;

    [MaxLength(100)]
    public string Name { get; set; } = "default";

    [MaxLength(100)]
    public string IconPath { get; set; } = "default";

    public Drawer? ParentDrawer { get; set; }

    public long ParentDrawerId { get; set; }
}
