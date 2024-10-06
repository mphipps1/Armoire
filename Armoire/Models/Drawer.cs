using System.Collections.Generic;
using Armoire.Interfaces;

namespace Armoire.Models;

public class Drawer : IDrawer
{
    public Drawer() { }

    public List<IContentsUnit> Contents { get; set; } = [];
    public int Id { get; set; }
}
