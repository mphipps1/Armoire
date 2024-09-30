using Armoire.Views;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.Interfaces
{
    public interface IDrawerDialogService
    {
        public void Btn_PointerReleased(object sender, PointerReleasedEventArgs e);

        public void ToggleDrawer(); 

    }
    public class DrawerDialog : IDrawerDialogService
    {
        public DrawerDialog(IDrawerDialogService drawerDialogService)
        {
        }

        public void Btn_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
           
        }

        public void ToggleDrawer()
        {
            
        }
    }
}
