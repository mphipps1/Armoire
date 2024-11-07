using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Armoire.Models;

public class Drawer
{
    // [Key] -> `Id` is the primary key.
    // NOTE: EF Core ignores read-only properties, so we need the `set`, even though we won't be
    // setting this property outside the constructor.
    // See: https://stackoverflow.com/a/43503578/16458003
    [Key]
    public long Id { get; set; }

    public Drawer() { }

    public Drawer? Parent { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = "default";

    [MaxLength(100)]
    public string ParentId { get; set; } = "default";

    public List<Drawer> Drawers { get; set; } = [];
    public List<Item> Items { get; set; } = [];
}
