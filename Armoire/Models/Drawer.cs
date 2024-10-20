using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Armoire.Interfaces;

namespace Armoire.Models;

public class Drawer : IDrawer, IContentsUnit
{
    // [Key] -> `Id` is the primary key.
    // [Database...] -> A new `Id` gets generated for each row inserted.
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public List<IContentsUnit> Contents { get; set; } = [];

    private string? _name;
    public string Name
    {
        get => _name ?? $"default{Id}";
        set => _name = value;
    }
    public string? IconPath { get; set; }
}
