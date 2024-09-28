using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels
{
    public partial class NewEntryPopUpViewModel : ViewModelBase
    {
        public NewEntryPopUpViewModel() { }
        public NewEntryPopUpViewModel(Item item)
        {
            Item = item;
        }

        private Item Item { get; set; }

        [ObservableProperty]
        public string _name;

        [ObservableProperty]
        public string _iconPath;

        [ObservableProperty]
        public string _path;

        public void UpdateItem()
        {
            Item.Name = Name;
            Item.IconPath = IconPath;
            Item.Path = Path;
        }

    }

}
