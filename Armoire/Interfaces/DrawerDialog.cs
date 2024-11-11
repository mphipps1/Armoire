using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Armoire.Views;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.Interfaces
{
    public interface IDrawerDialogService
    {
        public void Btn_PointerReleased(object sender, PointerReleasedEventArgs e);

        public void ToggleDrawer();
    }
}
