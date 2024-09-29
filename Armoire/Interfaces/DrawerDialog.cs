using Armoire.Views;
using Avalonia.Controls;
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
        public void DrawerDialogView();

    }
    public class DrawerDialog : IDrawerDialogService
    {
        public DrawerDialog() { 
        }

        public void DrawerDialogView()
        {
            throw new NotImplementedException();
        }

        /**
public void DrawerDialogView()
{
   DrawerDialogView dialog = new DrawerDialogView();

   var Windodialog = new Window
   {
       Content = dialog,
       Width = 300,
       Height = 200
   };

   Windodialog.ShowDialog(this);
}
*/
    }
}
