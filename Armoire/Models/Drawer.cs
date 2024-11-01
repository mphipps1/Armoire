using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Armoire.Models;

public class Drawer : ContentsUnit
{
    private static int _drawerId = 1;

    // [Key] -> `DrawerId` is the primary key.
    // NOTE: EF Core ignores read-only properties, so we need the `get` and the `set`, even though
    // we won't be setting this property outside the constructor.
    // See: https://stackoverflow.com/a/43503578/16458003
    [Key]
    public long DrawerId { get; set; }

    public Drawer()
    {
        DrawerId = _drawerId++;
    }

    public List<Drawer> Drawers { get; set; } = [];
    public List<Item> Items { get; set; } = [];
}
