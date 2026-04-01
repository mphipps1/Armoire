/*  An Item is defined as any unit in a drawer that can be clicked on and launches an app.
 *  An Item is usually an app, but it can also be a settings option or search for the weather
 * 
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Armoire.Models;
using Armoire.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Armoire.ViewModels;

public partial class ItemViewModel : ContentsUnitViewModel
{
    public string ExecutablePath { get; set; }
    public string? BmpName { get; set; }

    [ObservableProperty]
    public Avalonia.Media.Imaging.Bitmap _iconBmp;

    public ItemViewModel(string executablePath)
    {
        _iconBmp = GetIconForExec(executablePath);
        ExecutablePath = executablePath;
    }

    public ItemViewModel(string parentID, int? drawerHierarchy, ContainerViewModel? container)
    {
        ParentId = parentID;
        DrawerHierarchy = drawerHierarchy;
        Container = container;
        SetMoveDirections(this);
    }

    public ItemViewModel(
        string name,
        string executablePath,
        Avalonia.Media.Imaging.Bitmap bmp,
        string parentID,
        int? drawerHierarchy,
        ContainerViewModel? container,
        string? bmpName = null
    )
    {
        ExecutablePath = executablePath;
        Model = new Item(name, executablePath, parentID.ToString(), Position);
        Name = name;

        ParentId = parentID;
        DrawerHierarchy = drawerHierarchy;
        SetMoveDirections(this);
        
        IconBmp = GetIconForExec(executablePath); 
        Container = container;
        BmpName = bmpName;
    }

    public ItemViewModel(Item itemModel, ContainerViewModel container)
    {
        Id = itemModel.Id;
        //var modelIdCountStr = itemModel.Id[9..];
        //if (int.TryParse(modelIdCountStr, out var modelIdCount))
        //    IdCount = modelIdCount + 1;
        LoadPosition = itemModel.Position;
        ExecutablePath = itemModel.ExecutablePath;
        Model = new Item(itemModel);
        Name = itemModel.Name;
        ParentId = itemModel.ParentId;
        Container = container;
        DrawerHierarchy = itemModel.DrawerHierarchy;
        SetMoveDirections(this);
        IconBmp =
            GetIconForExec(ExecutablePath);
    }

    public ItemViewModel(ItemViewModel orig)
    {
        Id = orig.Id;
        ExecutablePath = orig.ExecutablePath;
        Model = orig.Model;
        Name = orig.Name;
        ParentId = orig.ParentId;
        Container = orig.Container;
        DrawerHierarchy = orig.DrawerHierarchy;
        SetMoveDirections(this);
        IconBmp =
            GetIconForExec(ExecutablePath);
    }

    public override void HandleContentsClick()
    {
        (Model as Item)?.Execute();
    }

    public Item CreateItem()
    {
        return new Item(
            Id,
            Name,
            ExecutablePath,
            ParentId ?? "MALFORMED_ITEM",
            Position,
            DrawerHierarchy
        );
    }
    /**
     * This function is made to take an file path to an executable application and return an avalonia bitmap containing
     * the application's icon.
     */
    public static Bitmap? GetIconForExec(string path)
    {
        if (OperatingSystem.IsWindows())
        {
            var icon = Icon.ExtractAssociatedIcon(path);
            var bmp = icon?.ToBitmap();
            if(bmp == null) return null;
                
            using var stream = new MemoryStream();
            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png); // System.Drawing.Bitmap has Save(Stream, ImageFormat)
            stream.Position = 0;
            return new Avalonia.Media.Imaging.Bitmap(stream);
        }
        else if (OperatingSystem.IsMacOS())
        {
            // On macOS applications are hosted in bundles. Within the plist file we will find a path to the icon file from within the bundle.
            var pListPath = Path.Combine(path, "Contents", "Info.plist");
            if(!File.Exists(pListPath)) return null;
            
            var plistContents = File.ReadAllText(pListPath);
            // get Icon file name from the plist that we found with the path above.
            var match = Regex.Match(plistContents, @"<key>CFBundleIconFile</key>\s*<string>(.*?)</string>");
            if(!match.Success) return null; //always fail if we cant find a valid iconFile.
            
            var iconFileName = match.Groups[1].Value;
            // we only want to add ".icns" if it is not there. This is for safety.
            if(!iconFileName.EndsWith(".icns"))
                iconFileName += ".icns";
            
            var icnsPath = Path.Combine(path,"Contents", "Resources", iconFileName);
            //generate a place to put the Png file after conversion.
            var temporaryPng = Path.GetTempFileName() + ".png";

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "sips",
                Arguments = $"-s format png \"{icnsPath}\" --out \"{temporaryPng}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                
            });
            //We need to wait for sips to finish before we try to load the new PNG
            process?.WaitForExit();
            
            //Now that temporary image should be populated we can load it into an avalonia bitmap.
            if (File.Exists(temporaryPng))
            {
                var aBmp = new Avalonia.Media.Imaging.Bitmap(temporaryPng);
                File.Delete(temporaryPng); //clean up by deleting the temp file.
                return aBmp;
            }
        }else if (OperatingSystem.IsLinux())
        {
            var appName = Path.GetFileNameWithoutExtension(path);
            var searchPaths = new[]
            {
                // with linux we are just checking all the places the app icon could live
                $"/usr/share/pixmaps/{appName}.png",
                $"/usr/share/icons/hicolor/256x256/apps/{appName}.png",
                $"/usr/share/icons/hicolor/128x128/apps/{appName}.png",
                $"/usr/share/icons/hicolor/64x64/apps/{appName}.png",
                Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.UserProfile), 
                    $".local/share/icons/{appName}.png")
            };
            //we don't need to convert since it's already a png.
            var iconPath = searchPaths.FirstOrDefault(File.Exists);
            if (iconPath != null)
                return new Avalonia.Media.Imaging.Bitmap(iconPath);
        }

        return null;
    }
}
