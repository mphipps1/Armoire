using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Armoire.Models;
using Armoire.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class ItemViewModel : ContentsUnitViewModel
{
    public string ExecutablePath { get; set; }
    public string? BmpName { get; set; }

    [ObservableProperty]
    public Avalonia.Media.Imaging.Bitmap _iconBmp;

    public ItemViewModel(string executablePath)
    {
        var bmp = Icon.ExtractAssociatedIcon(executablePath).ToBitmap();
        //which can be displayed by loading it into a memory stream to mimic downloading it
        using (MemoryStream memory = new MemoryStream())
        {
            bmp.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
        }
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
        System.Drawing.Bitmap bmp,
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
        //the following converts the System.Drawing.Bitmap which cannot
        //be displayed in a view to an Avalonia.Media.Imaging.Bitmap type
        //which can be displayed by loading it into a memory stream to mimic downloading it
        using (MemoryStream memory = new MemoryStream())
        {
            bmp.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
        }

        Container = container;
        BmpName = bmpName;
    }

    public ItemViewModel(Item itemModel, ContainerViewModel container)
    {
        Id = itemModel.Id;
        //var modelIdCountStr = itemModel.Id[9..];
        //if (int.TryParse(modelIdCountStr, out var modelIdCount))
        //    IdCount = modelIdCount + 1;
        ExecutablePath = itemModel.ExecutablePath;
        Model = new Item(itemModel);
        Name = itemModel.Name;
        ParentId = itemModel.ParentId;
        Container = container;
        DrawerHierarchy = itemModel.DrawerHierarchy;
        SetMoveDirections(this);
        IconBmp =
            MiscHelper.GetAvaBmpFromExePath(itemModel.ExecutablePath)
            ?? MiscHelper.GetAvaBmpFromAssets("exe_logo.png");
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
            MiscHelper.GetAvaBmpFromExePath(ExecutablePath)
            ?? MiscHelper.GetAvaBmpFromAssets("exe_logo.png");
    }

    public override void HandleContentsClick()
    {
        (Model as Item)?.Execute();
    }

    public Item CreateItem()
    {
        OutputHelper.DebugPrintJson(this, $"IVM-CreateItem-this-{Id}");
        return new Item(
            Id,
            Name,
            ExecutablePath,
            ParentId ?? "MALFORMED_ITEM",
            Position,
            DrawerHierarchy
        );
    }
}
