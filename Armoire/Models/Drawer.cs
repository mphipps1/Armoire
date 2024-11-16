using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Armoire.Models;

public class Drawer
{
    // [Key] -> `Id` is the primary key.
    // NOTE: EF Core ignores read-only properties, so we need the `set`, even though we won't be
    // setting this property outside the constructor.
    // See: https://stackoverflow.com/a/43503578/16458003
    [MaxLength(100)]
    [Key]
    public string Id { get; set; } = "default";

    // Parameterless constructor needed so EF can build the schema.
    public Drawer() { }

    public Drawer(string id, string name, string? parentId, int? position, int drawerHierarchy)
    {
        Id = id;
        Name = name;
        ParentId = parentId;
        Position = position;
        DrawerHierarchy = drawerHierarchy;
    }

    public Drawer? Parent { get; set; }

    public int? Position { get; set; }

    public int? DrawerHierarchy { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = "default";

    [MaxLength(100)]
    public string? ParentId { get; set; } = "default";

    public List<Drawer> Drawers { get; set; } = [];
    public List<Item> Items { get; set; } = [];
}
