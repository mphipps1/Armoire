using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Armoire.Interfaces;

public interface IItem : IContentsUnit
{
    // [Key] -> `Id` is the primary key.
    // [Database...] -> A new `Id` gets generated for each row inserted.
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ItemId { get; set; }

    public string ExecutablePath { get; set; }
}
