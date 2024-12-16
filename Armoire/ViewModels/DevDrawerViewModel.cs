/*  This was a temporary class used for displaying tools useful for development
 *  such as seeing the database
 */

using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class DevDrawerViewModel : ViewModelBase
{
    [ObservableProperty]
    private PixelPoint _point;

    public DevDrawerViewModel()
    {
        Point = new PixelPoint(230, 180);
    }
}
