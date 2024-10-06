using Armoire.Views;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.Interfaces
{
    public interface IDrawerDialogService
    {
        public void Btn_PointerReleased(object sender, PointerReleasedEventArgs e);

        public void ToggleDrawer(); 

    }
    public partial class DrawerDialog : ObservableObject, IDrawerDialogService
    {
        [ObservableProperty]
        private DrawerDialogView drawerDialogView;
        public DrawerDialog()
        {
         drawerDialogView = new DrawerDialogView();
           
        }

        public void Btn_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            drawerDialogView.Btn_PointerReleased(sender, e);
        }

        public void ToggleDrawer()
        {
            drawerDialogView.ToggleDrawer();   
        }
    }
}
