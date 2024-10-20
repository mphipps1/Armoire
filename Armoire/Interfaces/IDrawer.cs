using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Armoire.Interfaces;

public interface IDrawer
{
    List<IContentsUnit> Contents { get; set; }
}
