/*  ErrorMessage is used for displaying an error to the dialoghost
 * 
 */

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels
{
    public partial class ErrorMessageViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string _message;

        public ErrorMessageViewModel(string message)
        {
            _message = message;
        }
    }
}
