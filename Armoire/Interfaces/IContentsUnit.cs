using System.ComponentModel.DataAnnotations;

namespace Armoire.Interfaces;

public interface IContentsUnit
{
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string IconPath { get; set; }

    public IDrawer? ParentDrawer { get; set; }
    public long ParentDrawerId { get; set; }
}
