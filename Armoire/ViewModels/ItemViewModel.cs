﻿using System;
using System.IO;
using Armoire.Models;
using Armoire.Utils;

namespace Armoire.ViewModels;

public class ItemViewModel : ContentsUnitViewModel
{
    public string ExecutablePath { get; set; }

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
    }
}
