using System.Collections.ObjectModel;
using Armoire.Interfaces;

namespace Armoire.ViewModels;

public class DrawerViewModel : ViewModelBase, IHasId
{
    public ObservableCollection<ContentsUnitViewModel> Contents { get; set; } = [];
    public int Id { get; set; }

    public DrawerViewModel()
    {
        Contents.Add(new ItemViewModel());
        Contents.Add(new DrawerAsContentsViewModel());
        Contents.Add(new DrawerAsContentsViewModel());
    }
    public DrawerViewModel(int id)
    {
        Id = id;
    }
}
