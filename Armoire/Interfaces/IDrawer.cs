using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Armoire.Interfaces;

public interface IDrawer : IContentsUnit
{
    // [Key] -> `DrawerId` is the primary key.
    // [Database...] -> A new `DrawerId` gets generated for each row inserted.
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long DrawerId { get; set; }

    List<IDrawer> Drawers { get; set; }
    List<IItem> Items { get; set; }
}
