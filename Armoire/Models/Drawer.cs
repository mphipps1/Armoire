using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Armoire.Models;

public class Drawer : ContentsUnit
{
    // [Key] -> `DrawerId` is the primary key.
    // [Database...] -> A new `DrawerId` gets generated for each row inserted.
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long DrawerId { get; set; }

    public List<Drawer> Drawers { get; set; } = [];
    public List<Item> Items { get; set; } = [];
}
