using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Armoire.ViewModels;

namespace Armoire.Utils;

public class MiscHelper
{
    public static ItemViewModel CreateRandomItem(ContainerViewModel container)
    {
        if (
            NewItemViewModel.ExecutableNames is not { } exeNames
            || NewItemViewModel.Executables is not { } exes
            || NewItemViewModel.Icons is not { } icons
        )
            throw new InvalidOperationException(
                "Initialize `NewItemViewModel` static members before calling this method."
            );
        var rnd = new Random();
        var sampleItemIdx = rnd.Next(exeNames.Count);
        var sampleItemName = exeNames[sampleItemIdx];
        return new ItemViewModel(
            sampleItemName,
            exes[sampleItemName],
            icons[sampleItemName].ToBitmap(),
            container.SourceDrawerId
                ?? throw new InvalidOperationException(
                    "An `ItemViewModel` should not have a null `SourceDrawerId`."
                ),
            0,
            container
        );
    }

    public static string GetAssetsPath()
    {
        var workingDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var projectDirectoryDirInfo = Directory.GetParent(workingDirectory)?.Parent?.Parent?.Parent;

        if (projectDirectoryDirInfo is null)
            throw new InvalidOperationException("What?!");

        var assetsPath =
            projectDirectoryDirInfo
                .GetDirectories()
                .FirstOrDefault(dir => dir.Name == "Assets")
                ?.FullName ?? throw new InvalidOperationException("Que?!");

        return assetsPath;
    }

    public static void SetIconBmpOnDacVm(DrawerAsContentsViewModel dacVm)
    {
        var assetsPath = GetAssetsPath();
        const string drawerIconFilename = "tempDrawer.jpg";
        var fs = new FileStream(
            Path.Combine(assetsPath, drawerIconFilename),
            FileMode.Open,
            FileAccess.Read
        );
        var image = System.Drawing.Image.FromStream(fs);
        var bmp = new System.Drawing.Bitmap(image, 60, 60);
        using var memory = new MemoryStream();
        bmp.Save(memory, ImageFormat.Png);
        memory.Position = 0;
        dacVm.IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
    }
}
