using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Armoire.Models;
using Armoire.Utils;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;

namespace Armoire.ViewModels;

public partial class ItemViewModel : ContentsUnitViewModel
{
    public string ExecutablePath { get; set; }

    [ObservableProperty]
    public Avalonia.Media.Imaging.Bitmap _iconBmp;

    public ItemViewModel()
    {
        Name = "Paint";
        ExecutablePath = OsUtils.IsWindows11()
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Microsoft\WindowsApps\mspaint.exe"
            )
            : @"C:\WINDOWS\system32\mspaint.exe";
        IconKind = "Star";
        IconPath = "/Assets/mspaintLogo.svg";
        Model = new Item(Name, ExecutablePath, "0");
    }

    public ItemViewModel(string name, string executablePath, string parentID)
    {
        ExecutablePath = executablePath;
        Model = new Item(name, executablePath, parentID);
        Name = name;
    }

    public ItemViewModel(string name, string executablePath, string iconPath, string parentID)
    {
        ExecutablePath = executablePath;
        Model = new Item(name, executablePath, parentID);
        IconPath = iconPath;
        Name = name;
    }

    public ItemViewModel(string name, string executablePath, System.Drawing.Bitmap bmp, string parentID)
    {
        ExecutablePath = executablePath;
        Model = new Item(name, executablePath, parentID);
        Name = name;

        //the following converts the System.Drawing.Bitmap which cannot
        //be displayed in a view to an Avalonia.Media.Imaging.Bitmap type
        //which can be displayed by loading it into a memory stream to mimic downloading it
        using (MemoryStream memory = new MemoryStream())
        {
            bmp.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
        }
        
    }

    public override void HandleContentsClick()
    {
        (Model as Item)?.Execute();
    }
}
